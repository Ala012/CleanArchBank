using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Abstractions.Services;
using Application.Services;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddProblemDetails();

// Clean Architecture : la couche API ne connaît que les abstractions Application.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICompteService, CompteService>();
builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "CleanArchBank - API bancaire",
        Version = "v1",
        Description = "API de gestion de comptes bancaires en Clean Architecture : " +
                      "ASP.NET Core 10, EF Core 10 et MySQL 8 " +
                      "(provider Oracle MySql.EntityFrameworkCore, sans Pomelo)."
    });

    var xmlFile = Path.Combine(AppContext.BaseDirectory, "BankAPI.xml");
    if (File.Exists(xmlFile))
    {
        options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
    }
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(feature?.Error, "Erreur non gérée lors du traitement de {Path}", context.Request.Path);

        await Results.Problem(
            title: "Erreur interne du serveur",
            detail: app.Environment.IsDevelopment()
                ? feature?.Error?.Message
                : "Une erreur inattendue est survenue.",
            statusCode: StatusCodes.Status500InternalServerError)
            .ExecuteAsync(context);
    });
});

app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CleanArchBank v1");
        options.DocumentTitle = "CleanArchBank - API bancaire";
        options.DisplayRequestDuration();
    });
}

app.MapControllers();
app.MapHealthChecks("/health");
app.MapGet("/health/db", async (BankDbContext db, CancellationToken ct) =>
    await db.Database.CanConnectAsync(ct)
        ? Results.Ok(new { status = "UP", database = db.Database.GetDbConnection().Database })
        : Results.Problem("Base de données inaccessible.", statusCode: 503))
    .ExcludeFromDescription();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

// Migrations EF Core + jeu de données de démonstration au démarrage.
await app.Services.InitialiserBaseAsync();

app.Run();

/// <summary>Point d'entrée public, utile pour les tests d'intégration.</summary>
public partial class Program;
