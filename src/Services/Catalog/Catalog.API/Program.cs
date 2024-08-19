
using Marten;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
	config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddMarten( opts =>
{
	opts.Connection(builder.Configuration.GetConnectionString("Database")!);
	opts.AutoCreateSchemaObjects = Weasel.Core.AutoCreate.CreateOrUpdate; // Valor por defecto, no es necesario explícitamente
}).UseLightweightSessions() ;

var app = builder.Build();

app.MapCarter();

app.MapGet("/", () => "Hello World!");

app.Run();
