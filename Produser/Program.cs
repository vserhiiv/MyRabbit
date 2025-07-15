using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync("request-queue", exclusive: false);

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += (model, ea) =>
{
    Console.WriteLine($"Received Request: {ea.BasicProperties.CorrelationId}");

    var replyMessage = $"This is your reply: {ea.BasicProperties.CorrelationId}";

    var body = Encoding.UTF8.GetBytes(replyMessage);

    channel.BasicPublishAsync(string.Empty, ea.BasicProperties.ReplyTo!, body);

    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queue: "request-queue", autoAck: true, consumer: consumer);

Console.ReadKey();