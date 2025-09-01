using Discount.gRPC.Data;
using Discount.gRPC.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.gRPC.Services
{
    public class DiscountService(DiscountContext discountContext, ILogger<DiscountService> logger) : DiscountProtoService.DiscountProtoServiceBase
    {
        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            logger.LogInformation($"DiscountService.CreateDiscount");
            var coupon = request.Coupon.Adapt<Coupon>();

            if (coupon is null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalida request object"));
            }

            discountContext.Coupons.Add(coupon);
            await discountContext.SaveChangesAsync();

            logger.LogInformation($"DiscountService.CreateDiscount created sucessfully, Product Name: {coupon.ProductName}");

            return coupon.Adapt<CouponModel>();
        }
        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            logger.LogInformation($"DiscountService.GetDiscount");
            var coupon = await discountContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

            if(coupon == null)
            {
                //throw new Exception();
                coupon = new Models.Coupon { ProductName = "No discount", Amount = 0, Description = "None" };
            }

            logger.LogInformation($"DiscountService.GetDiscount retrieve {coupon}");

            var result = coupon.Adapt<CouponModel>();

            return result;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            logger.LogInformation($"DiscountService.CreateDiscount");
            //var coupon = request.Adapt<Coupon>();

            //if (coupon is null)
            //{
            //    throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalida request object"));
            //}

            var coupon = await discountContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.Coupon.ProductName);

            if(coupon is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Coupon not found"));
            }

            coupon.Description = request.Coupon.Description;
            coupon.Amount = request.Coupon.Amount;

            discountContext.Coupons.Update(coupon);
            await discountContext.SaveChangesAsync();

            logger.LogInformation($"DiscountService.CreateDiscount updated sucessfully, Product Name: {coupon.ProductName}");

            return coupon.Adapt<CouponModel>();
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var coupon = await discountContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

            if (coupon is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Coupon not found"));
            }

            discountContext.Coupons.Remove(coupon);
            await discountContext.SaveChangesAsync();

            logger.LogInformation($"DiscountService.DeleteDiscount deleted sucessfully, Product Name: {coupon.ProductName}");

            return new DeleteDiscountResponse() { Success = true };
        }
    }
}
