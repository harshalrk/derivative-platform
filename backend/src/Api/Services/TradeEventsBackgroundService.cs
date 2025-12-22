using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.AspNetCore.SignalR;
using Api.Hubs;

namespace Api.Services;

public class TradeEventsBackgroundService : BackgroundService
{
    private readonly ILogger<TradeEventsBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IModel? _channel;

    public TradeEventsBackgroundService(
        ILogger<TradeEventsBackgroundService> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TradeEventsBackgroundService starting...");

        try
        {
            // Connect to RabbitMQ
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://trader:trader123@rabbitmq:5672")
            };
            
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare queues (idempotent - won't recreate if exists)
            _channel.QueueDeclare(
                queue: "trade-created",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueDeclare(
                queue: "trade-updated",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _logger.LogInformation("Connected to RabbitMQ, listening for trade events");

            // Set up consumers
            await ConsumeTradeCreatedEvents(stoppingToken);
            await ConsumeTradeUpdatedEvents(stoppingToken);

            // Keep the service running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in TradeEventsBackgroundService");
            throw;
        }
    }

    private Task ConsumeTradeCreatedEvents(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var trade = JsonSerializer.Deserialize<object>(message);

                _logger.LogInformation("Received trade created event: {Message}", message);

                // Get hub context from DI
                using var scope = _serviceProvider.CreateScope();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<TradesHub>>();
                
                await hubContext.Clients.All.SendAsync("TradeCreated", trade, stoppingToken);
                _logger.LogInformation("Broadcasted TradeCreated event to all SignalR clients");
                
                _channel?.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing trade created event");
                _channel?.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel?.BasicConsume(
            queue: "trade-created",
            autoAck: false,
            consumer: consumer);

        return Task.CompletedTask;
    }

    private Task ConsumeTradeUpdatedEvents(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var trade = JsonSerializer.Deserialize<object>(message);

                _logger.LogInformation("Received trade updated event: {Message}", message);

                // Get hub context from DI
                using var scope = _serviceProvider.CreateScope();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<TradesHub>>();
                
                await hubContext.Clients.All.SendAsync("TradeUpdated", trade, stoppingToken);
                _logger.LogInformation("Broadcasted TradeUpdated event to all SignalR clients");
                
                _channel?.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing trade updated event");
                _channel?.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel?.BasicConsume(
            queue: "trade-updated",
            autoAck: false,
            consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
