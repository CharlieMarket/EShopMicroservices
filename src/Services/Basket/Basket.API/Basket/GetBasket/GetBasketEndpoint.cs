using Basket.API.Models;
using Carter;

namespace Basket.API.Basket.GetBasket
{
	public record GetBasketRequest();
	public record GetBasketResponse(ShoppingCart cart);
	public class GetBasketEndpoint : ICarterModule
	{
		public void AddRoutes(IEndpointRouteBuilder app)
		{
			app.MapGet("/basket/{usernamer}", async (Guid id, ISender sender) =>
			{

			}
		}
	}
}
