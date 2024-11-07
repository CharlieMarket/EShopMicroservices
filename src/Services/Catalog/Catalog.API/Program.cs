using System.Text.Json;
using BuildingBlocks.Behaviors;
using Catalog.API;
using Marten;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateSlimBuilder(args);

/// MUY IMPORTANTE: Por defecto ThrowOnBadRequest = true EN AMBIENTE DE DESARROLLO
/// Esto significa que todos los errores de binding de parámetros de GET y de DELETE 
/// y los de binding del body en POST y PUT (incluyendo formatos equivocados en GUID)
/// se mostrarán como BAD REQUEST con toda la info para el desarrollador
/// Si queremos que se comporte como en PRODUCCIÓN, donde no se crean excepciones, sino 
/// que se envía BAD REQUEST pero sin ningún tipo de información, para no extregar
/// información a un posible atacante, entonces usamos ThrowOnBadRequest = false
/// La siguiente línea se puede eliminar porque el ambiente indicará si se está en 
/// producción o no. Se deja para fines educativos y no complicarse al ver excepciones
/// pasando mientra desarrollamos
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = false);

var assembly = typeof(Program).Assembly;

// Register the ReadWriteDbContext
builder.Services.AddDbContext<ReadWriteDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("ReadWriteConnection")));

// Register the ReadOnlyDbContext
builder.Services.AddDbContext<ReadOnlyDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("ReadOnlyConnection")));

builder.Services.AddMediatR(config =>
{
	config.RegisterServicesFromAssembly(assembly);
	config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
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
//app.UseExceptionHandler(
//	exceptionHandlerApp =>
//{
//	exceptionHandlerApp.Run(async context =>
//	{
//		var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
//		if (exception == null)
//			return;
//		if (exception is BadHttpRequestException)
//			return;
//		var problemDetails = new ProblemDetails
//			{
//				Title = exception.Message,
//				Status = StatusCodes.Status500InternalServerError,
//				Detail = exception.StackTrace
//			};
//		context.Response.StatusCode = StatusCodes.Status500InternalServerError;

//		var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
//		logger.LogError(exception, exception.Message);

//		context.Response.ContentType = "application/problem+json";

//		await context.Response.WriteAsJsonAsync(problemDetails);

//	});
//});




app.MapGet("/", () => "Hello World!");

app.Run();
