using SoundStore.API.Extensions;
using SoundStore.API.Middlewares;
using SoundStore.Infrastructure;
using SoundStore.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Register(builder.Configuration);

builder.Services.RegisterServices();

builder.Services.RegisterInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();

app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "Sound Store v1");
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<UserClaimsMiddleware>();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
