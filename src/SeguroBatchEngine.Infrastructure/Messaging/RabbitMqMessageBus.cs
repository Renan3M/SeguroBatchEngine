using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using SeguroBatchEngine.Domain.Interfaces;

namespace SeguroBatchEngine.Infrastructure.Messaging;

public class RabbitMqMessageBus : IMessageBus, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    
    public const string QueueName = "apolices.fallback.ai";
    public const string DlqExchangeName = "apolices.dlx";
    public const string DlqQueueName = "apolices.fallback.ai.dlq";

    private RabbitMqMessageBus(IConnection connection, IChannel channel)
    {
        _connection = connection;
        _channel = channel;
    }

    public static async Task<RabbitMqMessageBus> CreateAsync(
        string hostName = "localhost",
        string userName = "admin",
        string password = "admin")
    {
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        // Dead Letter Exchange & Queue
        await channel.ExchangeDeclareAsync(DlqExchangeName, ExchangeType.Direct, durable: true);
        await channel.QueueDeclareAsync(DlqQueueName, durable: true, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync(DlqQueueName, DlqExchangeName, routingKey: QueueName);

        // Fila Principal com redirecionamento para DLQ em caso de falha permanente
        var args = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", DlqExchangeName },
            { "x-dead-letter-routing-key", QueueName }
        };

        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: args);

        return new RabbitMqMessageBus(connection, channel);
    }

    public async Task PublicarFallbackAsync(ApoliceFallbackEvent evento, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(evento);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: QueueName,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.CloseAsync();
        await _connection.CloseAsync();
    }
}