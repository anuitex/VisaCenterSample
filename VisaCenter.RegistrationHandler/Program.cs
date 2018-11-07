using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;
using VisaCenter.AppHandlers;
using VisaCenter.DomainEvents;
using VisaCenter.Repository;
using VisaCenter.Repository.Models;
using VisaCenter.Repository.Repositories;


namespace VisaCenter.RegistrationHandler
{
    class Program
    {
        static void Main(string[] args)
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

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);


                    //handle message
                    var visa = JsonConvert.DeserializeObject<Visa>(message);

                    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                    optionsBuilder.UseSqlServer("Data Source=KARFAGEN;Database=VisaCenter;Trusted_Connection=True;MultipleActiveResultSets=true", providerOptions => providerOptions.CommandTimeout(60));

                    var handler = new VisaCheckHandler(new VisaRepository(new ApplicationDbContext(optionsBuilder.Options), null));
                    handler.HandleAsync(new VisaRegistredEvent { Visa = visa }, null);
                };
                channel.BasicConsume(queue: "reg_q_1",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
