using AuxiliumSoftware.AuxiliumServices.Common.Configuration.Sections.Databases.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Configuration.Sections.Databases
{
    public class RabbitMQConfigurationSection
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; } = 0;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string VirtualHost { get; set; } = null!;
        public int HeartbeatIntervalInSeconds { get; set; } = 0;
        public int BlockedConnectionTimeoutInSeconds { get; set; } = 0;
        public string ExchangeName { get; set; } = null!;
        public RabbitMQQueuesConfigurationSection Queues { get; set; } = null!;



        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Host))            throw new InvalidOperationException("Configuration value 'Databases->RabbitMQ->Host' is missing.");
            if (Port <= 0)                                  throw new InvalidOperationException("Configuration value 'Databases->RabbitMQ->Port' is missing or invalid.");
            if (string.IsNullOrWhiteSpace(Username))        throw new InvalidOperationException("Configuration value 'Databases->RabbitMQ->Username' is missing.");
            if (string.IsNullOrWhiteSpace(Password))        throw new InvalidOperationException("Configuration value 'Databases->RabbitMQ->Password' is missing.");
            if (string.IsNullOrWhiteSpace(VirtualHost))     throw new InvalidOperationException("Configuration value 'Databases->RabbitMQ->VirtualHost' is missing.");
            if (HeartbeatIntervalInSeconds <= 0)            throw new InvalidOperationException("Configuration value 'Databases->RabbitMQ->HeartbeatInSeconds' is missing or invalid.");
            if (BlockedConnectionTimeoutInSeconds <= 0)     throw new InvalidOperationException("Configuration value 'Databases->RabbitMQ->BlockedConnectionTimeoutInSeconds' is missing or invalid.");
            if (string.IsNullOrWhiteSpace(ExchangeName))    throw new InvalidOperationException("Configuration value 'Databases->RabbitMQ->ExchangeName' is missing.");

            Queues.Validate();
        }
    }
}
