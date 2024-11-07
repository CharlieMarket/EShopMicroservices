﻿
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>
        (ILogger<LoggingBehavior<TRequest, TResponse>> logger) 
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            logger.LogInformation("[Start] handle request={Request} - Response={Response} - RequestData={RequestData}",
                typeof(TRequest).Name, typeof(TResponse).Name, request);
            var timer = new Stopwatch();
            timer.Start();

            var response = await next();
            timer.Stop();
            var timeTaken = timer.Elapsed;
            if (timeTaken.Seconds > 3) // SI demora más de 3 segundos
                logger.LogWarning("[Performance] The request {Request} took {TimeTaken} seconds",
                    typeof(TRequest).Name, timeTaken.Seconds);

            logger.LogInformation("[End] handle {Request} with {Response}",
                typeof(TRequest).Name, typeof(TResponse).Name);

            return response;
        }
    }
}