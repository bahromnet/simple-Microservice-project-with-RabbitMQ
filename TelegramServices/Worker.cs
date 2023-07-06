using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Telegram.Bot;

namespace TelegramServices;

public class Worker : BackgroundService
{
    static TelegramBotClient _bot = new TelegramBotClient("bot_token_here");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var factory = new ConnectionFactory
            {
                HostName = "10.10.1.181",
                UserName = "Javlon",
                Password = "Javlon"
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: "Bahrom.botservice.get",
                                autoAck: true,
                                consumer: consumer);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received {message}");

                var chatId = "chat_id_here";
                await _bot.SendTextMessageAsync(chatId, message);

                var body2 = Encoding.UTF8.GetBytes($"Raxmat, {message} => xabarini oldim");
                channel.BasicPublish(
                        exchange: "Bahrom.botservice.exchange",
                        routingKey: "bahrom.botservice.send",
                        basicProperties: null,
                        body: body2);
            };

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
