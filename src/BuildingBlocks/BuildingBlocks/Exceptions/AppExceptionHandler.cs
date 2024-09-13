using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BuildingBlocks.Exceptions
{
	public static class GlobalExceptionHandler {
		public static async Task HandleExceptionAsync(HttpContext context)
		{
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = StatusCodes.Status500InternalServerError;

			var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
			var exception = exceptionHandlerFeature?.Error;

			var response = new
			{
				Message = "An unexpected error occurred. Please try again later.",
				Details = exception?.Message // Consider logging this to a file or monitoring system
			};

			var jsonResponse = JsonSerializer.Serialize(response);
			await context.Response.WriteAsync(jsonResponse);
		}
	}

}