using SoundStore.API.Extensions;
using SoundStore.Infrastructure;
using SoundStore.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Register();

builder.Services.RegisterServices(builder.Configuration);

builder.Services.RegisterInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
