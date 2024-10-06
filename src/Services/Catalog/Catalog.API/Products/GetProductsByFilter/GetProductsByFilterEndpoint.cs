
using BuildingBlocks.EndpointFilters;
using Marten;

namespace Catalog.API.Products.GetProductsByFilter
{
	public class GetProductsByFilterRequest {
		public Guid? Id { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public string? Category { get; set; }
		public string? ImageFile { get; set; }
		public decimal? MinimunPrice { get; set; }
		public decimal? MaximunPrice { get; set; }
		public ProductFilterType? FilterType { get; set; }
	}

	public class GetProductsByFilterRequestValidator : AbstractValidator<GetProductsByFilterRequest>
	{
        public GetProductsByFilterRequestValidator()
        {
			RuleFor(r => r).Must(r => {
				ProductFilterType ft = ProductFilterType.None;
				ft = (r.Id is null || r.Id == Guid.Empty) ? ft : (ft | ProductFilterType.AllowId);
				ft = (string.IsNullOrEmpty(r.Name)) ? ft : (ft | ProductFilterType.AllowPartialName);
				ft = (string.IsNullOrEmpty(r.Category)) ? ft : (ft | ProductFilterType.Category);
				ft = (r.MinimunPrice == null) ? ft : (ft | ProductFilterType.MinimunPrice);
				ft = (r.MaximunPrice == null) ? ft : (ft | ProductFilterType.MaximunPrice);
				r.FilterType = ft;

				return ft.IsOneOf(ProductFilterType.AllowId,
						 ProductFilterType.AllowPartialName,
						 ProductFilterType.AllowPartialNamePriceRange,
						 ProductFilterType.AllowCategoryMaximunPrice);
			}).WithMessage("filter combination not allowed!"); ;
		}
    }

	public record GetProductsByFilterResponse(Product[] Products);
	public class GetProductsByFilterEndpoint : ICarterModule
	{
		public void AddRoutes(IEndpointRouteBuilder app)
		{
			app.MapGet("/products/filter", async ([AsParameters] GetProductsByFilterRequest request, ISender sender) =>
			{
				var result = await sender.Send(request.Adapt<GetProductsByFilterQuery>());
				var response = result.Adapt<GetProductsByFilterResponse>();
				return Results.Ok(response);
			})
			.WithName("GetProductsByFilter")
			.Produces<GetProductsByFilterResponse>(StatusCodes.Status200OK)
			.WithSummary("Get Products by Filter")
			.WithDescription("Get Products by Filter")
			.WithRequestValidation<GetProductsByFilterRequest>();
		}
	}

}

