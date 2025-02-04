using Application;
using Infrastructure;
using Infrastructure.Data;
using WebApi;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPresentationServices(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

app.UseCors("Angular Client");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Events API v1");
        options.OAuthClientId("angular");
    });

    await app.InitialiseDatabaseAsync();
}

app.UseIdentityServer();
app.UseTokenVersionValidation();
app.UseAuthorization();

app.MapControllers();

app.Run();