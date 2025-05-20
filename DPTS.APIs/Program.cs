using DPTS.Applications;
var builder = WebApplication.CreateBuilder(args);

builder.Services.Initalize(builder.Configuration);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
