using DPTS.Applications;
using DPTS.Infrastructures.Data;
var builder = WebApplication.CreateBuilder(args);

builder.Services.Initalize(builder.Configuration);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "DPTS.APIs", Version = "v1" });
    c.EnableAnnotations();
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(options =>
    {
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PM.Identity.API v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "PM.Identity.API v2");
        c.RoutePrefix = string.Empty;
    });
    app.UseStaticFiles();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();


app.Run();
