using Sys.Application;
var builder = WebApplication.CreateBuilder(args);

builder.Services.Initialize()
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
