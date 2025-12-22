using Persistence;
using Persistence.Projections;
using Pricing;
using Models.Events;
using Models.Aggregates;
using Marten;
using Marten.Events.Projections;
using Marten.Events.Daemon.Resiliency;
using Api.Hubs;
using Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SignalR
builder.Services.AddSignalR();

// Configure CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:7051")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configure Marten with Event Sourcing
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Database=traderdb;Username=trader;Password=trader123";
builder.Services.AddMarten(opts =>
{
    opts.Connection(connectionString);
    
    // Enable event sourcing with string-based stream identity
    opts.Events.StreamIdentity = Marten.Events.StreamIdentity.AsString;
    
    // Register swap trade event types
    opts.Events.AddEventType(typeof(SwapTradeCreated));
    opts.Events.AddEventType(typeof(SwapTradeUpdated));
    opts.Events.AddEventType(typeof(TradePriced));
    opts.Events.AddEventType(typeof(TradeCancelled));
    
    // Configure async projection for read models
    opts.Projections.Add<TradeProjection>(ProjectionLifecycle.Async);
    
    // Configure aggregate snapshots
    opts.Projections.Snapshot<SwapTradeAggregate>(SnapshotLifecycle.Inline);
}).AddAsyncDaemon(DaemonMode.HotCold);

// Configure Rebus (disabled for MVP due to dependency conflicts)
// var rabbitMqConnection = "amqp://trader:trader123@localhost:5672";
// builder.Services.AddRebusMessaging(rabbitMqConnection);

// Register repositories and services
builder.Services.AddScoped<ITradeRepository, TradeRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
builder.Services.AddScoped<IPricingService, PricingService>();

// Register background service for RabbitMQ -> SignalR
builder.Services.AddHostedService<TradeEventsBackgroundService>();

var app = builder.Build();

// Seed configuration data on startup
using (var scope = app.Services.CreateScope())
{
    var configRepo = scope.ServiceProvider.GetRequiredService<IConfigurationRepository>();
    await configRepo.SeedConfigurationDataAsync();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();
app.MapHub<TradesHub>("/hubs/trades");

app.Run();
