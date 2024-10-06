using Marten;

namespace Catalog.API.Products.GetProductByCategory
{
	public record GetProductByCategoryQuery(String Category) : IQuery<GetProductByCategoryResult>;
	public record GetProductByCategoryResult(IEnumerable<Product> Products);
	internal class GetProductByCategoryQueryHandler
		(IDocumentSession session, ILogger<GetProductByCategoryQueryHandler> logger)
		: IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
	{
		public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery query, CancellationToken cancellationToken)
		{
			logger.LogInformation("GetProductByCategoryResult.Handle called with {@query}", query);

			var products = await session.Query<Product>()
				.Where(p => p.Category.Contains(query.Category))
				.ToListAsync(cancellationToken);

			return new GetProductByCategoryResult(products);
		}
	}
}
