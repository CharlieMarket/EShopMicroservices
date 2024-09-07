using BuildingBlocks.EndpointFilters;
using Catalog.API.Products.CreateProduct;
using Catalog.API.Products.GetProductsByFilter;

namespace Catalog.API.Products.UpdateProduct
{
	public record UpdateProductRequest(Guid Id, string Name, string Description, List<string> Category, string ImageFile, decimal Price);

	public class UpdateProductRequestvalidator : AbstractValidator<UpdateProductRequest>
	{
		public UpdateProductRequestvalidator()
		{
			RuleFor(x => x.Id).NotEmpty().WithMessage("Product id is required!");
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("name is required!")
				.Length(2, 150).WithMessage("name must be between 2 and 150 characters!");
			RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greather than 0!");
		}
	}
	public record UpdateProductResponse(bool IsSuccess);
	public class UpdateProductEndpoint : ICarterModule
	{
		public void AddRoutes(IEndpointRouteBuilder app)
		{
			app.MapPut("/products",
				async (UpdateProductRequest request, ISender sender) =>
				{
					var command = request.Adapt<UpdateProductCommand>();
					var result = await sender.Send(command);
					var response = result.Adapt<UpdateProductResponse>();
					return Results.Ok(response);
				}
			)
			.WithName("UpdateProduct")
			.Produces<UpdateProductResponse>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.WithSummary("Update Product")
			.WithDescription("Update Product")
			.WithRequestValidation<UpdateProductRequest>();

		}
	}
}
