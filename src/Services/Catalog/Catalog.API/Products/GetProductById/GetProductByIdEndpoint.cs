namespace Catalog.API.Products.GetProductById
{
	// public record GetProductByIdRequest(Guid Id);
	public record GetProductByIdResponse(Product Product);
	public class GetProductByIdEndpoint : ICarterModule
	{
		public void AddRoutes(IEndpointRouteBuilder app)
		{
			app.MapGet("/products/{id}", async (Guid id, ISender sender) =>
			{
				var result = await sender.Send(new GetProductByIdQuery(id));
				var respuesta = result.Product.Match<IResult>(
					product => {
						var r = new GetProductByIdResponse(product.Adapt<Product>());
						return Results.Ok(r);
						},
					notFound => Results.NotFound($"Not found product with Id ${notFound.Id}")
				);
				return respuesta;
			})
			.WithName("GetProductById")
			.Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Get Product by Id")
			.WithDescription("Get Product by Id");
		}
	}
}
