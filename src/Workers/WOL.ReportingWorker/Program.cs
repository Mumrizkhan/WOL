using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using MongoDB.Driver;
using Serilog;
using WOL.ReportingWorker;
using WOL.ReportingWorker.Jobs;
using WOL.ReportingWorker.Services;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSerilog();

var mongoConnectionString = builder.Configuration["MongoDB:ConnectionString"] ?? "mongodb://localhost:27017";
var mongoDatabaseName = builder.Configuration["MongoDB:DatabaseName"] ?? "wol_reporting";
var mongoClient = new MongoClient(mongoConnectionString);
var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);

builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);
builder.Services.AddScoped<IReportingService, ReportingService>();

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(
        builder.Configuration.GetConnectionString("HangfireConnection"))));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<DailyReportGenerationJob>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BookingCompletedReportConsumer>();
    x.AddConsumer<PaymentProcessedReportConsumer>();

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

RecurringJob.AddOrUpdate<DailyReportGenerationJob>(
    "daily-report-generation",
    job => job.Execute(),
    Cron.Daily);

host.Run();
