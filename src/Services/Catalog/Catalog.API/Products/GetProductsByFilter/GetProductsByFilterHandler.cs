namespace Catalog.API.Products.GetProductsByFilter
{
	public enum ProductFilterType
	{
		None = 0,
		AllowId = 1,
		AllowPartialName = 1 << 1,
		Category = 1 << 2,
		MinimunPrice = 1 << 3,
		MaximunPrice = 1 << 4,
		AllowPartialNamePriceRange = AllowPartialName | MinimunPrice | MaximunPrice,
		AllowCategoryMaximunPrice = Category | MaximunPrice
	}
	public record GetProductsByFilterQuery
		(Guid Id, string Name, string Description, List<string> Category, string ImageFile, decimal BeginPrice, decimal EndPrice, ProductFilterType FilterType) 
		: IQuery<GetProductsByFilterResult>;
	public record GetProductsByFilterResult(IEnumerable<Product> Products);

	internal class GetProductsByFilterQueryHandler(IDocumentSession session, ILogger<GetProductsByFilterQueryHandler> logger)
		: IQueryHandler<GetProductsByFilterQuery, GetProductsByFilterResult>
	{
		public async Task<GetProductsByFilterResult> Handle(GetProductsByFilterQuery query, CancellationToken cancellationToken)
		{
			logger.LogInformation("GetProductsByFilterQueryHandler.Handler called with {@query}", query);
			var product = await session.LoadAsync<Product>(query.Id, cancellationToken);
			

			var products = await session.Query<Product>()
			.Where(p =>
				(string.IsNullOrEmpty(query.Name) || p.Name.Contains(query.Name)) &&
				(string.IsNullOrEmpty(query.Description) || p.Description.Contains(query.Description)) &&
				(query.Category == null || query.Category.Count == 0 || p.Category.Any(c => query.Category.Contains(c)))
			).ToListAsync(cancellationToken);

			return new GetProductsByFilterResult(products);
		}
	}
}