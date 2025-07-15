using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

var replyQueue = await channel.QueueDeclareAsync(queue: string.Empty, exclusive: true);
await channel.QueueDeclareAsync("request-queue", exclusive: false);

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (model, ea) =>
{
    await Task.Delay(3000);

    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Reply recieved: {message}");
};

await channel.BasicConsumeAsync(queue: replyQueue.QueueName, autoAck: true, consumer: consumer);

for (int i = 0; i < 3; i++)
{
    var message = "Can I request reply";
    var body = Encoding.UTF8.GetBytes(message);

    var properties = new BasicProperties
    {
        CorrelationId = Guid.NewGuid().ToString(),
        ReplyTo = replyQueue.QueueName
    };

    await channel.BasicPublishAsync(
        exchange: string.Empty,
        routingKey: "request-queue",
        mandatory: true,
        basicProperties: properties,
        body: body);

    Console.WriteLine($"Sending Request: {properties.CorrelationId}");

    await Task.Delay(1000);
}

Console.ReadKey();