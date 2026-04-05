using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using StudentManagement.API.Extensions;
using StudentManagement.API.Middleware;
using StudentManagement.Infrastructure.Data;

// ─── Serilog Bootstrap Logger ───────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Student Management API...");

    var builder = WebApplication.CreateBuilder(args);

    // ─── Serilog Full Logger ─────────────────────────────────────────────────
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.File("logs/studentmgmt-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
    });

    // ─── Services ────────────────────────────────────────────────────────────
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddDatabase(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddSwaggerWithJwt();
    builder.Services.AddApplicationServices();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
    });

    // ─── Build App ───────────────────────────────────────────────────────────
    var app = builder.Build();

    // ─── Auto Migrate & Seed DB ──────────────────────────────────────────────
    // using (var scope = app.Services.CreateScope())
    // {
    //     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //     db.Database.Migrate();
    //     Log.Information("Database migration applied successfully");
    // }
 using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var retries = 10;
    while (retries > 0)
    {
        try
        {
            db.Database.EnsureCreated();   // 🔥 REQUIRED
            db.Database.Migrate();

            Log.Information("Database migration applied successfully");
            break;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Migration failed, retrying...");
            retries--;
            Thread.Sleep(5000);
        }
    }
}
    // ─── Middleware Pipeline ─────────────────────────────────────────────────
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Management API v1");
            c.RoutePrefix = string.Empty; // Swagger at root
            c.DocumentTitle = "Student Management API";
            c.DisplayRequestDuration();
        });
    }

    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Student Management API started on {Urls}", string.Join(", ", builder.WebHost.GetSetting("urls") ?? "default ports"));

app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
public partial class Program { }
