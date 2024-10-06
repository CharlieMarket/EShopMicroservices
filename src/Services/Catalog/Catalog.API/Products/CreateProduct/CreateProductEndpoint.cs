using BuildingBlocks.EndpointFilters;
using Microsoft.AspNetCore.Diagnostics;

namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductRequest(string Name, string Description, List<string> Category, string ImageFile, decimal Price);

	public class CreateProductRequestvalidator : AbstractValidator<CreateProductRequest>
	{
		public CreateProductRequestvalidator()
		{
			RuleFor(x => x.Name).NotEmpty().WithMessage("name is required!");
			RuleFor(x => x.Category).NotEmpty().WithMessage("Category is required!");
			RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile is required!");
			RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greather than 0!");
		}
	}

	public record CreateProductResponse(Guid Id);
	public class CreateProductEndpoint : ICarterModule
	{
		public void AddRoutes(IEndpointRouteBuilder app)
		{
			app.MapPost("/products",
				async (CreateProductRequest request, ISender sender) =>
				{
					var command = request.Adapt<CreateProductCommand>();
					var result = await sender.Send(command);
					var response = result.Adapt<CreateProductResponse>();
					return Results.Created($"/productos/{response.Id}", response);
				}
			)
   			.WithRequestValidation<CreateProductRequest>()
			.WithName("CreateProduct")
			.Produces<CreateProductResponse>(StatusCodes.Status201Created)
			.WithSummary("Create Product")
			.WithDescription("Create Product");
			
		}
	}
}
