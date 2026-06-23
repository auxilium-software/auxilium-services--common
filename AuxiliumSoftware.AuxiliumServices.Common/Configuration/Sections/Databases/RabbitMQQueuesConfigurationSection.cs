using System;
using System.Collections.Generic;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Configuration.Sections.Databases
{
    public class RabbitMQQueuesConfigurationSection
    {
        public string DeadLetter { get; set; } = null!;
        public string Notifications { get; set; } = null!;



        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(DeadLetter))      throw new InvalidOperationException("Configuration value 'Databases->RabbitMQ->Queues->DeadLetter' is missing.");
            if (string.IsNullOrWhiteSpace(Notifications))   throw new InvalidOperationException("Configuration value 'Databases->RabbitMQ->Queues->Notifications' is missing.");
        }
    }
}
