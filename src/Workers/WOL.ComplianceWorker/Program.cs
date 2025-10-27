using MassTransit;
using Serilog;
using WOL.ComplianceWorker;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSerilog();

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
host.Run();
