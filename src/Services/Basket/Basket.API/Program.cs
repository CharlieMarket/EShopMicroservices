var builder = WebApplication.CreateBuilder(args);

// Add services to teh containers

var app = builder.Build();

// Configure the HPP request pipeline

app.MapGet("/", () => "Hello Basket API!");

app.Run();
