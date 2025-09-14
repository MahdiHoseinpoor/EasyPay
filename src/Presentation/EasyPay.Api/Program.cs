using Asp.Versioning;
using EasyPay.Api.Extensions;
using EasyPay.Api.Middleware;
using EasyPay.Infrastructure.Data;
using Serilog;
const string BlazorAppCorsPolicy = "BlazorAppCorsPolicy";
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
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: BlazorAppCorsPolicy,
                          policy =>
                          {
                              policy.WithOrigins("http://localhost:5038")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                          });
    });
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
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
    app.UseCors(BlazorAppCorsPolicy);
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