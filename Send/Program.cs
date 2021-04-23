using System;
using System.IO;
using RabbitMQ.Client;
using RabbitMQ.Util;
using System.Text;

namespace Send
{
  class Send
  {
    public static void Main()
    {
      var factory = new ConnectionFactory() {
        UserName = "test",
        Password = "test",
        HostName = "master.sharenj.org",
        VirtualHost = "jkalil",
      };

      factory.Ssl.Version = System.Security.Authentication.SslProtocols.Tls12;

      factory.Ssl.ServerName = "master.sharenj.org";
      factory.Ssl.CertPath = @"C:\code\api\rabbitmq-client-dotnet\Certificate\server.pem";
      factory.Ssl.Enabled = true;
      factory.Port = 5671;

      using(IConnection connection = factory.CreateConnection())
      using(IModel channel = connection.CreateModel())
      {
        Console.WriteLine("Successfully connected and opened a channel");

        channel.QueueDeclare(queue: "TestAppQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        Console.WriteLine("Successfully declared a queue");

        Console.WriteLine("Writing message...");
        string message = "More Hello World More Hello World More Hello World with Ssl and consumer with ssl";
        var body = Encoding.UTF8.GetBytes(message);

        Console.WriteLine("Sending message...");
        channel.BasicPublish(exchange: "", routingKey: "TestAppQueue", basicProperties: null, body: body);

        Console.WriteLine(" [x] Sent {0}", message);
      }

      Console.WriteLine(" Press [enter] to exit.");
      Console.ReadLine();
    }
  }
}
