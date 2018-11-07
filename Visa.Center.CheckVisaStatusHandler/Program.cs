using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using VisaCenter.Repository;
using VisaCenter.Repository.Repositories;

namespace Visa.Center.CheckVisaStatusHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "rpc_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);


                consumer.Received += async (model, ea) =>
                {
                    string response = null;

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var id = int.Parse(Encoding.UTF8.GetString(body));

                        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                        optionsBuilder.UseSqlServer("Data Source=KARFAGEN;Database=VisaCenter;Trusted_Connection=True;MultipleActiveResultSets=true", providerOptions => providerOptions.CommandTimeout(60));
                        var repository = new VisaRepository(new ApplicationDbContext(optionsBuilder.Options), null);
                        var v = (await repository.FindAsync(x => x.Id == id)).FirstOrDefault();
                        response = v.VisaStatus;
                    }
                    catch (Exception e)
                    {
                        //log
                        response = "";
                    }
                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                          basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                          multiple: false);
                    }
                };
                channel.BasicConsume(queue: "rpc_queue", autoAck: false, consumer: consumer);
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
