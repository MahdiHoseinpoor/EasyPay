using EasyPay.Api.Extensions;
using EasyPay.Api.Middleware;
using EasyPay.Infrastructure.Data;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateLogger();

try
{
    Log.Information("Starting EasyPay API host");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();
    builder.AddApplicationServices();
    builder.AddInfrastructureServices();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddMemoryCache();
    builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();
    builder.Services.AddControllers();
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<ApplicationDbContext>("database");
    builder.Services.AddIdentityServices(builder.Configuration);
    builder.Services.AddSwaggerDocumentation();

    var app = builder.Build();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyPay API V1");
            c.RoutePrefix = string.Empty;
        });
    }
    app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapCustomHealthChecks();
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