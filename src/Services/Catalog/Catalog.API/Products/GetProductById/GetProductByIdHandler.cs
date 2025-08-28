using Catalog.API.Models;
using Marten;
using Marten.Linq.QueryHandlers;
using UniversalCommon.CQRS;

namespace Catalog.API.Products.GetProductById
{
    public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;
    public record GetProductByIdResult(Product Product);

    public class GetProductByIdQueryValidator: AbstractValidator<GetProductByIdQuery>
    {
        public GetProductByIdQueryValidator()
        {
            RuleFor(q => q.Id).NotEmpty().WithMessage("Id is required");
        }
    }
    internal class GetProductByIdQueryHandler(IDocumentSession session) : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
    {
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            //var result = await session.Query<Product>().FirstOrDefaultAsync(x => x.Id == query.Id);
            var result = await session.LoadAsync<Product>(query.Id, cancellationToken);

            if (result == null) {
                throw new ProductNotFoundException(query.Id);
            }

            return new GetProductByIdResult(result);
        }
    }
}
