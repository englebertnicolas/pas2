using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PAS.ActuarialEngine.Persistence;
using PAS.AspireServiceDefaults;
using PAS.AspNetCore.Configuration;
using PAS.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var thisAssembly = typeof(Program).Assembly;
var rabbitMqCnc = builder.Configuration.GetConnectionString("RabbitMq") ?? throw new InvalidOperationException("RabbitMq connection string not found.");
var dbCnc = builder.Configuration.GetConnectionString("Database") ?? throw new InvalidOperationException("Database connection string not found.");

builder
    .AddAspireServiceDefaults()
    .SetDefaultCulture();

builder.Services
    .AddProblemDetails()
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddHttpContextAccessor()
    .AddValidatorsFromAssembly(thisAssembly)
    .AddDefaultOpenApi()
    .AddDefaultWolverine(dbCnc, ActuDbContext.SchemaName, rabbitMqCnc, builder.Environment.IsDevelopment(), [thisAssembly])
    .AddDbContext<ActuDbContext>(options => options.UseSqlServer(dbCnc), ServiceLifetime.Scoped, ServiceLifetime.Singleton);

var app = builder.Build();
app.ConfigureHttpResultConverter();
app.UseExceptionHandler();
app.UseDefaultOpenApi("PAS.ActuarialEngine API Reference");
app.UseHttpsRedirection();

app.MapDefaultEndpoints();
app.MapEndpointFromAssembly(thisAssembly);
app.Run();
