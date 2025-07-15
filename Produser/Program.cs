using RabbitMQ.Client;
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

var message = "But how can one possibly pay attention to a book with no pictures in it?";

var body = Encoding.UTF8.GetBytes(message);

await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "hello", body: body);
Console.WriteLine($" Published message: {message}");
Console.ReadKey();
