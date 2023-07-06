using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMQLesson;

internal class Program
{
    static async Task Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Enter message....");
            Console.Write(">> ");
            string str = Console.ReadLine()!;
            FanoutTest(str);
        }
    }

    static void FanoutTest(string str)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "10.10.1.181",
            UserName = "Javlon",
            Password = "Javlon"
        };

        using var connection = factory.CreateConnection();
        using var model = connection.CreateModel();

        //const string message = "Salomlar Dunyolar Hellolar";

        var body = Encoding.UTF8.GetBytes(str);

        model.BasicPublish(
                exchange: "Bahrom.botservice.exchange",
                routingKey: "bahrom.botservice.get",
                basicProperties: null,
                body: body
            );

        Console.WriteLine("Kettik....");
        Console.ReadKey();
    }

    static void HeaderTest(string str)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "bahrom",
            Password = "admin"
        };

        using var connection = factory.CreateConnection();
        using var model = connection.CreateModel();

        var prop = model.CreateBasicProperties();

        Dictionary<string, object> proporties = new();
        proporties.Add("Username", "Ali");
        proporties.Add("Token", "warning123");
        proporties.Add("Title", "warning");

        prop.Headers = proporties;

        var body = Encoding.UTF8.GetBytes(str);
        model.BasicPublish("header.test", "info.header", prop, body);

        Console.WriteLine("Message Sent From : header.test ");
        Console.WriteLine("Routing Key : Does not need routing key");
        Console.WriteLine("Message Sent");
        Console.ReadKey();
    }

    static void TopicTest()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "bahrom",
            Password = "admin"
        };
        using var connection = factory.CreateConnection();
        using var model = connection.CreateModel();

        var body = Encoding.UTF8.GetBytes("Topicka Dunyodan Salomlar");

        model.BasicPublish("topic.test", "info.info", null, body);

        Console.WriteLine("Kettik....");
        Console.ReadKey();
    }

}
