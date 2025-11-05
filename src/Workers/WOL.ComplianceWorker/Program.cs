using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using MongoDB.Driver;
using Serilog;
using WOL.ComplianceWorker;
using WOL.ComplianceWorker.Jobs;
using WOL.ComplianceWorker.Services;
using WOL.Compliance.Infrastructure.Data;
using WOL.Compliance.Infrastructure.Repositories;
using WOL.Compliance.Domain.Repositories;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSerilog();

builder.Services.AddDbContext<ComplianceDbContext>();

builder.Services.AddScoped<IComplianceRecordRepository, ComplianceRecordRepository>();

var mongoConnectionString = builder.Configuration["MongoDB:ConnectionString"] ?? "mongodb://localhost:27017";
var mongoDatabaseName = builder.Configuration["MongoDB:DatabaseName"] ?? "wol_compliance";
var mongoClient = new MongoClient(mongoConnectionString);
var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);

builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);
builder.Services.AddScoped<IComplianceService, ComplianceService>();

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(
        builder.Configuration.GetConnectionString("HangfireConnection"))));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<DocumentExpiryCheckJob>();
builder.Services.AddScoped<ComplianceMonitoringJob>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<VehicleRegisteredComplianceConsumer>();
    x.AddConsumer<DocumentExpiringComplianceConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();

RecurringJob.AddOrUpdate<DocumentExpiryCheckJob>(
    "document-expiry-check",
    job => job.Execute(),
    Cron.Daily); // Daily at midnight

RecurringJob.AddOrUpdate<ComplianceMonitoringJob>(
    "compliance-monitoring",
    job => job.Execute(),
    Cron.Daily); // Daily at midnight

host.Run();
