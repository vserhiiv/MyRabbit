using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

var queueDeclare = await channel.QueueDeclareAsync() ?? throw new Exception("Declaring the queue was unsuccessful");

var queueName = queueDeclare.QueueName;


await channel.QueueBindAsync(queue: queueName, exchange: "pubsub", routingKey: string.Empty);

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Message Received: {message}");

    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine("Consuming...");

Console.ReadKey();
