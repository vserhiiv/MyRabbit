using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "mytopicgexchange", ExchangeType.Topic);

var queueDeclare = await channel.QueueDeclareAsync();

var queueName = queueDeclare.QueueName;

await channel.QueueBindAsync(queue: queueName, exchange: "mytopicgexchange", routingKey: "user.#");

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Users - received new message: {message}");

    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine("Users - Consuming...");

Console.ReadKey();
