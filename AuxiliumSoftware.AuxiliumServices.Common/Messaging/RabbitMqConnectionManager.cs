using AuxiliumSoftware.AuxiliumServices.Common.Configuration.Sections.Databases;
using AuxiliumSoftware.AuxiliumServices.Common.Messaging.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Messaging
{
    public class RabbitMqConnectionManager : IRabbitMqConnectionManager
    {
        private readonly RabbitMQConfigurationSection _configuration;
        private readonly ILogger<RabbitMqConnectionManager> _logger;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);

        private IConnection? _connection;
        private bool _disposed;

        public RabbitMqConnectionManager(
            RabbitMQConfigurationSection configuration,
            ILogger<RabbitMqConnectionManager> logger
        )
        {
            _configuration = configuration;
            _logger = logger;
        }

        public RabbitMQConfigurationSection Configuration => _configuration;

        public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
        {
            if (_connection is { IsOpen: true })
                return _connection;

            await _connectionLock.WaitAsync(cancellationToken);
            try
            {
                if (_connection is { IsOpen: true })
                    return _connection;

                _logger.LogInformation(
                    "Establishing RabbitMQ connection to {Host}:{Port}/{VirtualHost}",
                    _configuration.Host, _configuration.Port, _configuration.VirtualHost);

                var factory = new ConnectionFactory
                {
                    HostName = _configuration.Host,
                    Port = _configuration.Port,
                    UserName = _configuration.Username,
                    Password = _configuration.Password,
                    VirtualHost = _configuration.VirtualHost,
                    RequestedHeartbeat = TimeSpan.FromSeconds(_configuration.HeartbeatIntervalInSeconds),
                    RequestedConnectionTimeout = TimeSpan.FromSeconds(_configuration.BlockedConnectionTimeoutInSeconds),
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                };

                _connection = await factory.CreateConnectionAsync(cancellationToken);

                _logger.LogInformation("RabbitMQ connection established successfully");

                return _connection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to establish RabbitMQ connection");
                throw;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            if (_connection is { IsOpen: true })
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }

            _connectionLock.Dispose();
            _disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
