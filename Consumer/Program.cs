using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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

await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global:  false);

var consumer = new AsyncEventingBasicConsumer(channel);

var random = new Random();

consumer.ReceivedAsync += async (model, ea) =>
{
    var processingTime = random.Next(1, 6);

    var body = ea.Body.ToArray();

    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Message Received: {message} will take {processingTime} to process");

    Task.Delay(TimeSpan.FromSeconds(processingTime)).Wait();

    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
};

await channel.BasicConsumeAsync(queue: "hello", autoAck: false, consumer: consumer);

Console.WriteLine("Consuming...");

Console.ReadKey();