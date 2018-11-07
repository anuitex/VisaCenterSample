using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VisaCenter.DomainEvents;
using VisaCenter.Interfaces.Handlers;

namespace VisaCenter.RabbitMqHandlers
{
    public class RabbitMqVisaCheckHandler : IEventHandler<VisaRegistredEvent, object>
    {
        public Task<object> HandleAsync(VisaRegistredEvent ev, IBus bus)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "reg_q_1",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = JsonConvert.SerializeObject(ev.Visa);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "reg_q_1",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            return null;
        }
    }
}
