using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "myroutingexchange", ExchangeType.Direct);

var queueDeclare = await channel.QueueDeclareAsync();

var queueName = queueDeclare.QueueName;

await channel.QueueBindAsync(queue: queueName, exchange: "myroutingexchange", routingKey: "analyticsonly");

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Analytics - received new message: {message}");

    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine("Analytics - Consuming...");

Console.ReadKey();