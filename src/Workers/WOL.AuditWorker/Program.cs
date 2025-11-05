using MassTransit;
using MongoDB.Driver;
using Serilog;
using WOL.AuditWorker;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSerilog();

var mongoConnectionString = builder.Configuration["MongoDB:ConnectionString"] ?? "mongodb://localhost:27017";
var mongoDatabaseName = builder.Configuration["MongoDB:DatabaseName"] ?? "wol_audit";

builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnectionString));
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDatabaseName);
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AuditLogCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.ReceiveEndpoint("audit-log-queue", e =>
        {
            e.ConfigureConsumer<AuditLogCreatedConsumer>(context);
            e.PrefetchCount = 16;
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        });
    });
});

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
    var collection = database.GetCollection<AuditLogDocument>("audit_logs");
    
    var indexKeysDefinition = Builders<AuditLogDocument>.IndexKeys
        .Ascending(x => x.UserId)
        .Ascending(x => x.Timestamp);
    
    await collection.Indexes.CreateOneAsync(new CreateIndexModel<AuditLogDocument>(indexKeysDefinition));
    
    var actionIndexKeysDefinition = Builders<AuditLogDocument>.IndexKeys.Ascending(x => x.Action);
    await collection.Indexes.CreateOneAsync(new CreateIndexModel<AuditLogDocument>(actionIndexKeysDefinition));
    
    var timestampIndexKeysDefinition = Builders<AuditLogDocument>.IndexKeys.Descending(x => x.Timestamp);
    await collection.Indexes.CreateOneAsync(new CreateIndexModel<AuditLogDocument>(timestampIndexKeysDefinition));
    
    Log.Information("MongoDB indexes created successfully");
}

Log.Information("Audit Worker Service starting...");
await host.RunAsync();
