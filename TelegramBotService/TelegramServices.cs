using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Telegram.Bot;

public class TelegramServices : BackgroundService
{
    private readonly TelegramBotClient _bot;
    private readonly IConnection _connection;
    private readonly IModel _model;

    public TelegramServices()
    {
        _bot = new TelegramBotClient("6258639001:AAFDjpTwuIQZxEBHf1XgJvaD7yDKemkPll0");
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "bahrom",
            Password = "admin"
        };
        _connection = factory.CreateConnection();
        _model = _connection.CreateModel();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_model);
        consumer.Received += async (sender, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                await ReportMessageToBroker(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
            finally
            {
                _model.BasicAck(ea.DeliveryTag, false);
            }
        };

        _model.BasicConsume(queue: "fanout.info", autoAck: false, consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await base.StopAsync(stoppingToken);
        _model?.Close();
        _model?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }

    private async Task ReportMessageToBroker(string message)
    {
        var chatId = "969446629";
        await _bot.SendTextMessageAsync(chatId, message);
    }
}
