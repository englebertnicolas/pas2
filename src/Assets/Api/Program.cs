using Microsoft.EntityFrameworkCore;
using PAS.AspireServiceDefaults;
using PAS.AspNetCore.Configuration;
using PAS.AspNetCore.Diagnostics;
using PAS.Assets.Persistence;

var builder = WebApplication.CreateBuilder(args);
var rabbitMqCnc = builder.Configuration.GetConnectionString("RabbitMq") ?? throw new InvalidOperationException("RabbitMq connection string not found.");
var dbCnc = builder.Configuration.GetConnectionString("Database") ?? throw new InvalidOperationException("Database connection string not found.");

builder
    .AddAspireServiceDefaults()
    .SetDefaultCulture();

builder.Services
    .AddDefaultProblemDetails()
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddDefaultOpenApi()
    .AddDefaultWolverine(dbCnc, AssetDbContext.SchemaName, rabbitMqCnc, builder.Environment.IsDevelopment(), [typeof(Program).Assembly])
    .AddDbContext<AssetDbContext>(options => options.UseSqlServer(dbCnc), ServiceLifetime.Scoped, ServiceLifetime.Singleton);

var app = builder.Build();
app.UseExceptionHandler();
app.UseDefaultOpenApi("PAS.Assets API Reference");
app.UseHttpsRedirection();
app.MapDefaultEndpoints();
app.MapEndpointFromAssembly(typeof(Program).Assembly);
app.Run();
