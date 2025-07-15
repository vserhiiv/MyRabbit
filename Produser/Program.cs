using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "mytopicgexchange", type: ExchangeType.Topic);

var random = new Random();

var messageId = 1;

while (true)
{
    var publishingTime = random.Next(1, 4);

    var userPaymentMessage = $"A european user paid for something: {messageId}";

    var userPaymentBody = Encoding.UTF8.GetBytes(userPaymentMessage);

    await channel.BasicPublishAsync(exchange: "mytopicgexchange", routingKey: "user.europe.payments", body: userPaymentBody);
    
    Console.WriteLine($"This message needs to be routed: {userPaymentMessage}");


    var businessOrderMessage = $"A european bussiness ordered goods: {messageId}";
    
    var businessOrderBody = Encoding.UTF8.GetBytes(businessOrderMessage);
    
    await channel.BasicPublishAsync(exchange: "mytopicgexchange", routingKey: "business.europe.order", body: businessOrderBody);
    
    Console.WriteLine($"This message needs to be routed: {businessOrderMessage}");

    Task.Delay(TimeSpan.FromSeconds(publishingTime)).Wait();

    messageId++;
}
