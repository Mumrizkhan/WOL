using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using WOL.Booking.Infrastructure.Data;
using WOL.Booking.Infrastructure.Repositories;
using WOL.Booking.Domain.Repositories;
using WOL.BookingWorker.Jobs;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting Booking Worker Service");

    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddSerilog();

    builder.Services.AddDbContext<BookingDbContext>();

    builder.Services.AddScoped<IBookingRepository, BookingRepository>();

    builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString("HangfireConnection"))));

    builder.Services.AddHangfireServer();

    builder.Services.AddScoped<DriverAssignmentTimeoutJob>();

    builder.Services.AddMassTransit(x =>
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(builder.Configuration["RabbitMQ:Host"], h =>
            {
                h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
                h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
            });

            cfg.ConfigureEndpoints(context);
        });
    });

    var host = builder.Build();

    RecurringJob.AddOrUpdate<DriverAssignmentTimeoutJob>(
        "driver-assignment-timeout-check",
        job => job.Execute(),
        "* * * * *"); // Every minute

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Booking Worker Service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
