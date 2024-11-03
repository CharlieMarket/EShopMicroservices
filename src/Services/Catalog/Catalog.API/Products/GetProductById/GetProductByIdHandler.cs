using BuildingBlocks.CommandErrors;
using Marten;
using OneOf;

namespace Catalog.API.Products.GetProductById
{
	public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;
	public record GetProductByIdResult(OneOf<Product, RecordNotFound> Product);
	internal class GetProductByIdQueryHandler(IDocumentSession session, ILogger<GetProductByIdQueryHandler> logger) 
		: IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
	{
		public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
		{
			logger.LogInformation("GetProductByIdQueryHandler.Handler called with {@query}", query);
			var product = await session.LoadAsync<Product>(query.Id, cancellationToken);

			if(product is null)
			{
				return new GetProductByIdResult(new RecordNotFound(query.Id));
			}

			return new GetProductByIdResult(product);
		}
	}
}
