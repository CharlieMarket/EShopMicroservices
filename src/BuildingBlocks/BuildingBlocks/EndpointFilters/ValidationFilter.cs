using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EndpointFilters
{
	public class ValidationFilter<TRequest> : IEndpointFilter
	{
		private readonly IValidator<TRequest> validator;

		public ValidationFilter(IValidator<TRequest> validator)
		{
			this.validator = validator;
		}

		public async ValueTask<object?> InvokeAsync(
			EndpointFilterInvocationContext context,
			EndpointFilterDelegate next)
		{
			var request = context.Arguments.OfType<TRequest>().First();

			var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);

			if (!result.IsValid)
			{
				return TypedResults.ValidationProblem(result.ToDictionary());
			}

			return await next(context);
		}
	}

	public static class ValidationExtensions
	{
		public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
		{
			return builder.AddEndpointFilter<ValidationFilter<TRequest>>()
				.ProducesValidationProblem();
		}
	}
}


