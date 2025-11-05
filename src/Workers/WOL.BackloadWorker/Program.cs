using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using MongoDB.Driver;
using Serilog;
using WOL.BackloadWorker;
using WOL.BackloadWorker.Jobs;
using WOL.BackloadWorker.Services;
using WOL.Backload.Infrastructure.Data;
using WOL.Backload.Infrastructure.Repositories;
using WOL.Backload.Domain.Repositories;
using WOL.Booking.Infrastructure.Data;
using WOL.Booking.Infrastructure.Repositories;
using WOL.Booking.Domain.Repositories;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSerilog();

builder.Services.AddDbContext<BackloadDbContext>();
builder.Services.AddDbContext<BookingDbContext>();

builder.Services.AddScoped<IRouteUtilizationRepository, RouteUtilizationRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

var mongoConnectionString = builder.Configuration["MongoDB:ConnectionString"] ?? "mongodb://localhost:27017";
var mongoDatabaseName = builder.Configuration["MongoDB:DatabaseName"] ?? "wol_backload";
var mongoClient = new MongoClient(mongoConnectionString);
var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);

builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);
builder.Services.AddScoped<IBackloadMatchingService, BackloadMatchingService>();

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(
        builder.Configuration.GetConnectionString("HangfireConnection"))));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<RouteUtilizationAggregationJob>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BookingCreatedBackloadConsumer>();
    x.AddConsumer<BookingCompletedBackloadConsumer>();

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

RecurringJob.AddOrUpdate<RouteUtilizationAggregationJob>(
    "route-utilization-aggregation",
    job => job.Execute(),
    Cron.Hourly);

host.Run();
