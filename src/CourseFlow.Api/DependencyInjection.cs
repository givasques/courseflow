using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CourseFlow.Api;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        return builder;
    }
    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSnakeCaseNamingConvention();

            if(builder.Environment.IsDevelopment())
            {
                options
                    .UseSqlite(
                        connectionString,
                        sqliteOptions => sqliteOptions
                            .MigrationsHistoryTable(HistoryRepository.DefaultTableName));
            } 
            else
            {
                options
                    .UseNpgsql(
                        connectionString,
                        npgsqlOptions => npgsqlOptions
                            .MigrationsHistoryTable(HistoryRepository.DefaultTableName));
            }
        });

        
        builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
        {
            options.UseSnakeCaseNamingConvention();

            if(builder.Environment.IsDevelopment())
            {
                options
                    .UseSqlite(
                        connectionString,
                        sqliteOptions => sqliteOptions
                            .MigrationsHistoryTable(HistoryRepository.DefaultTableName));
            } 
            else
            {
                options
                    .UseNpgsql(
                        connectionString,
                        npgsqlOptions => npgsqlOptions
                            .MigrationsHistoryTable(HistoryRepository.DefaultTableName));
            }
        });

        return builder;
    }

    public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>();

        return builder;
    }

    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
        .WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddConsoleExporter();    
        })
        .WithMetrics(metrics =>
        {
            metrics
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter();
        });

        return builder;
    }
}
