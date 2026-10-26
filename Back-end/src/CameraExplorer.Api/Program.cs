using CameraExplorer.Core;
using CameraExplorer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// The CSV path can be overridden with configuration ("Cameras:CsvPath"); the default is the copy next to the app.
var csvPath = builder.Configuration["Cameras:CsvPath"]
    ?? Path.Combine(AppContext.BaseDirectory, "data", "cameras-defb.csv");

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173"];

builder.Services.AddSingleton<ICameraRepository>(new CsvCameraRepository(csvPath));
builder.Services.AddSingleton<CameraService>();

builder.Services.AddCors(options =>
    options.AddPolicy("Frontend", policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Load the data now so a missing file fails at startup, and log any rows that were skipped.
var cameraService = app.Services.GetRequiredService<CameraService>();
foreach (var issue in cameraService.GetLoadIssues())
{
    app.Logger.LogWarning(
        "Skipped line {LineNumber} of the camera data ('{Line}'): {Reason}.",
        issue.LineNumber, issue.Line, issue.Reason);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");

app.MapGet("/api/cameras", (string? name, CameraService service) => Results.Ok(service.Search(name)))
   .WithName("GetCameras")
   .WithSummary("Returns all cameras, optionally filtered by a case-insensitive part of the name.");

app.Run();

// Makes the Program class visible to the integration tests.
public partial class Program { }