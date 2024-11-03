using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text.Json;
using System.Text.Json.Serialization;

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
		(Guid Id, string Name, string Description, string Category, string ImageFile, decimal MinimunPrice, decimal MaximunPrice, ProductFilterType FilterType) 
		: IQuery<GetProductsByFilterResult>;
	public record GetProductsByFilterResult(IEnumerable<Product> Products);

	public class ProductDataResult
	{
		public string Data { get; set; } = string.Empty;

		//[JsonIgnore]
		//public Product? ProductData
		//{
		//	get => JsonSerializer.Deserialize<Product>(Data);
		//	set => Data = JsonSerializer.Serialize(value);
		//}
	}

	internal class GetProductsByFilterQueryHandler(ReadOnlyDbContext dbContext, ILogger<GetProductsByFilterQueryHandler> logger)
		: IQueryHandler<GetProductsByFilterQuery, GetProductsByFilterResult>
	{
		public async Task<GetProductsByFilterResult> Handle(GetProductsByFilterQuery query, CancellationToken cancellationToken)
		{
			logger.LogInformation("GetProductsByFilterQueryHandler.Handler called with {@query}", query);

			List<Product?> products = [];
			
			switch (query.FilterType)
			{
				case ProductFilterType.AllowCategoryMaximunPrice:
					var sql = "SELECT * FROM get_products_by_Category_MaxPrice(@category, @maxPrice)";
					var categoryParam = new NpgsqlParameter("@category", query.Category);
					var maxPriceParam = new NpgsqlParameter("@maxPrice", query.MaximunPrice);

					var jsonProducts = await dbContext.ProductDataResults.FromSqlRaw(sql, categoryParam, maxPriceParam).ToListAsync(cancellationToken);
					products = jsonProducts.Select(p => JsonSerializer.Deserialize<Product>(p.Data)).Where(pd => pd != null).ToList();
					break;
				case ProductFilterType.AllowPartialName:
					sql = "SELECT * FROM get_products_by_PartialName(@name)";
					var nameParam = new NpgsqlParameter("@name", query.Name);

					jsonProducts = await dbContext.ProductDataResults.FromSqlRaw(sql, nameParam).ToListAsync(cancellationToken);
					products = jsonProducts.Select(p => JsonSerializer.Deserialize<Product>(p.Data)).Where(pd => pd != null).ToList();
					break;

			}
			var returnedProducts = new GetProductsByFilterResult([.. products]);
			return returnedProducts;
		}
	}
}
/*

CREATE OR REPLACE FUNCTION get_products_by_Category_MaxPrice(p_category TEXT, p_max_price NUMERIC)
RETURNS TABLE(data JSONB) AS $$
BEGIN
    RETURN QUERY
    SELECT m.data
    FROM mt_doc_product m
    WHERE 
        m.data->'Category' ? p_category
        AND (m.data->>'Price')::NUMERIC <= p_max_price;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION get_products_by_PartialName(p_name TEXT)
RETURNS TABLE(data JSONB) AS $$
BEGIN
    RETURN QUERY
    SELECT m.data
    FROM mt_doc_product m
    WHERE 
        (m.data->'Name')::TEXT ILIKE '%' || p_name || '%';
END;
$$ LANGUAGE plpgsql;

-- Index on the 'Category' array within 'data'
CREATE INDEX idx_product_category ON your_table_name USING GIN ((data->'Category'));

-- Index on the 'Price' field within 'data'
CREATE INDEX idx_product_price ON your_table_name ((data->>'Price')::NUMERIC);
 
 */
