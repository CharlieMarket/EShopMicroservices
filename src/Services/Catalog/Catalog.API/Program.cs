using BuildingBlocks.Behaviors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateSlimBuilder(args);

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
	config.RegisterServicesFromAssembly(assembly);
	config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddCarter();

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddMarten( opts =>
{
	opts.Connection(builder.Configuration.GetConnectionString("Database")!);
	opts.AutoCreateSchemaObjects = Weasel.Core.AutoCreate.CreateOrUpdate; // Valor por defecto, no es necesario explícitamente
}).UseLightweightSessions() ;

builder.WebHost.UseKestrelHttpsConfiguration(); //opcional, como estamos detrás de un gateway, no necesitamos https

var app = builder.Build();



app.MapCarter();
//app.UseExceptionHandler(options => { });
// ERRORES DE BINDING: OJO: NO DEJAR PASAR EL STACK TRACE
//  BadHttpRequestException: error al enlazar el parámetro "Nullable<int> pageNumber" a partir de "two".
//  BadHttpRequestException: Failed to bind parameter "Nullable<int> pageNumber" from "two". 412
app.UseExceptionHandler(
	exceptionHandlerApp =>
{
	exceptionHandlerApp.Run(async context =>
	{
		var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
		if (exception == null)
			return;

		var problemDetails = new ProblemDetails
		{
			Title = exception.Message,
			Status = StatusCodes.Status500InternalServerError,
			Detail = exception.StackTrace
		};
		context.Response.StatusCode = StatusCodes.Status500InternalServerError;

		if (exception.Message.Contains("bind parameter") || exception.Message.Contains("enlazar el parámetro"))
		{
			problemDetails.Title = "Invalid parameter value";
			problemDetails.Status = StatusCodes.Status412PreconditionFailed;
			problemDetails.Detail = "Parameter not match required format";
			context.Response.StatusCode = StatusCodes.Status412PreconditionFailed;
		}

		var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
		logger.LogError(exception, exception.Message);

	

		context.Response.ContentType = "application/problem+json";

		await context.Response.WriteAsJsonAsync(problemDetails);

	});
});

app.MapGet("/", () => "Hello World!");

app.Run();
