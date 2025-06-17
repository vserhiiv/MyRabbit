using RabbitMQ.Client;
using System;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(
    queue: "hello",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var random = new Random();

var messageId = 1;

while (true)
{
    var publishingTime = random.Next(1, 4);

    var message = $"Sending messageId: {messageId}";

    var body = Encoding.UTF8.GetBytes(message);

    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "hello", body: body);

    Console.WriteLine($" Published message: {message}");

    Task.Delay(TimeSpan.FromSeconds(publishingTime)).Wait();

    messageId++;
}
