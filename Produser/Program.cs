using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "pubsub", type: ExchangeType.Fanout);

var random = new Random();

var messageId = 1;

while (true)
{
    var publishingTime = random.Next(1, 4);

    var message = $"Message: {messageId}";

    var body = Encoding.UTF8.GetBytes(message);

    await channel.BasicPublishAsync(exchange: "pubsub", routingKey: string.Empty, body: body);

    Console.WriteLine($" Published message: {message}");

    Task.Delay(TimeSpan.FromSeconds(publishingTime)).Wait();

    messageId++;
}
