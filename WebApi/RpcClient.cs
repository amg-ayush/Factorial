using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Text;

namespace WebApi
{
    /// <summary>
    /// Вспомогательный класс для связывания двух сервисов через RabbitMQ
    /// </summary>
    public class RpcClient
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string queueName;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties props;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="queueName">Название очереди</param>
        public RpcClient(string queueName)
        {
            this.queueName = queueName;

            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            string correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = "amq.rabbitmq.reply-to";

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                    respQueue.Add(response);
            };
        }

        /// <summary>
        /// Подсчёт значения факториала
        /// </summary>
        /// <param name="numberText">Число в текстовом формате</param>
        /// <returns>Подсчитанное значение результата</returns>
        public string CalculateFactorial(string numberText)
        {
            var bytes = Encoding.UTF8.GetBytes(numberText);

            channel.BasicConsume(
                consumer: consumer,
                queue: "amq.rabbitmq.reply-to",
                autoAck: true);

            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: props,
                body: bytes);

            return respQueue.Take();
        }

        /// <summary>
        /// Закрытие соединения
        /// </summary>
        public void Close()
        {
            connection.Close();
        }
    }
}
