using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using WOL.Payment.Infrastructure.Data;
using WOL.Payment.Infrastructure.Repositories;
using WOL.Payment.Domain.Repositories;
using WOL.PaymentWorker.Jobs;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting Payment Worker Service");

    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddSerilog();

    builder.Services.AddDbContext<PaymentDbContext>();

    builder.Services.AddScoped<IPaymentDeadlineRepository, PaymentDeadlineRepository>();

    builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString("HangfireConnection"))));

    builder.Services.AddHangfireServer();

    builder.Services.AddScoped<PaymentDeadlineCheckJob>();

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

    RecurringJob.AddOrUpdate<PaymentDeadlineCheckJob>(
        "payment-deadline-check",
        job => job.Execute(),
        "* * * * *"); // Every minute

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Payment Worker Service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
