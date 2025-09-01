using Discount.gRPC.Models;
using Microsoft.EntityFrameworkCore;

namespace Discount.gRPC.Data
{
    public class DiscountContext : DbContext
    {
        public DbSet<Coupon> Coupons { get; set; }

        public DiscountContext(DbContextOptions<DiscountContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coupon>().HasData(
                new Coupon { Id = 1, Amount = 10, Description = "10%", ProductName = "Prod1" },
                new Coupon { Id = 2, Amount = 20, Description = "20%", ProductName = "Prod2" }
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
