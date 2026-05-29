using AuxiliumSoftware.AuxiliumServices.Common.Messaging.Interfaces;
using AuxiliumSoftware.AuxiliumServices.Common.Messaging.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AuxiliumSoftware.AuxiliumServices.Common.Messaging
{
    public class RabbitMqProducer : IMessageQueueProducer
    {
        private readonly IRabbitMqConnectionManager _connectionManager;
        private readonly ILogger<RabbitMqProducer> _logger;
        private readonly SemaphoreSlim _channelLock = new(1, 1);

        private IChannel? _channel;
        private bool _exchangeDeclared;
        private bool _disposed;

        public RabbitMqProducer(
            IRabbitMqConnectionManager connectionManager,
            ILogger<RabbitMqProducer> logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
        }

        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : QueueMessage
        {
            ArgumentNullException.ThrowIfNull(message);

            var channel = await GetOrCreateChannelAsync(cancellationToken);

            var json = JsonSerializer.Serialize(message, message.GetType(), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                MessageId = message.MessageId.ToString(),
                CorrelationId = message.CorrelationId,
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                Timestamp = new AmqpTimestamp(((DateTimeOffset)message.CreatedAt).ToUnixTimeSeconds()),
                Type = typeof(T).Name
            };

            await channel.BasicPublishAsync(
                exchange: _connectionManager.Configuration.ExchangeName,
                routingKey: message.RoutingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogDebug(
                "Published message {MessageId} with routing key {RoutingKey}",
                message.MessageId, message.RoutingKey);
        }

        private async Task<IChannel> GetOrCreateChannelAsync(CancellationToken cancellationToken)
        {
            if (_channel is { IsOpen: true } && _exchangeDeclared)
                return _channel;

            await _channelLock.WaitAsync(cancellationToken);
            try
            {
                if (_channel is { IsOpen: true } && _exchangeDeclared)
                    return _channel;

                var connection = await _connectionManager.GetConnectionAsync(cancellationToken);
                _channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

                await _channel.ExchangeDeclareAsync(
                    exchange: _connectionManager.Configuration.ExchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    cancellationToken: cancellationToken);

                _exchangeDeclared = true;

                return _channel;
            }
            finally
            {
                _channelLock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            if (_channel is { IsOpen: true })
            {
                await _channel.CloseAsync();
                _channel.Dispose();
            }

            _channelLock.Dispose();
            _disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
