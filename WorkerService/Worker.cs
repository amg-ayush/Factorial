using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using WorkerService.Utils;

namespace WorkerService
{
    /// <summary>
    /// Воркер считающий значение факториала
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private const string queueName = "InQueue";

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="logger">Логгер</param>
        public Worker(ILogger<Worker> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000, stoppingToken);

                ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };

                using (IConnection connection = factory.CreateConnection())
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: queueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    channel.BasicQos(0, 1, false);

                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                    consumer.Received += (model, ea) =>
                    {
                        string response = string.Empty;
                        var body = ea.Body.ToArray();
                        var props = ea.BasicProperties;
                        var replyProps = channel.CreateBasicProperties();
                        replyProps.CorrelationId = props.CorrelationId;
                        try
                        {
                            var message = Encoding.UTF8.GetString(body);
                            response = long.Parse(message).GetFactorialText();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(" [.] " + e.Message);
                        }
                        finally
                        {
                            var responseBytes = Encoding.UTF8.GetBytes(response);
                            channel.BasicPublish(
                                exchange: "",
                                routingKey: props.ReplyTo,
                                basicProperties: replyProps,
                                body: responseBytes);
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        }
                    };

                    Console.WriteLine(" [x] Awaiting RPC requests");
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}