using BibliotecaApi.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<EstudiantesServices>();
builder.Services.AddScoped<LibrosService>();
builder.Services.AddScoped<PrestamosService>();
builder.Services.AddScoped<DevolucionesService>();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider
        .GetRequiredService<EstudiantesServices>()
        .InitializeAsync();

    await scope.ServiceProvider
        .GetRequiredService<LibrosService>()
        .InitializeAsync();

    await scope.ServiceProvider
        .GetRequiredService<PrestamosService>()
        .InitializeAsync();

    await scope.ServiceProvider
        .GetRequiredService<DevolucionesService>()
        .InitializeAsync();
}
app.MapOpenApi();

app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
