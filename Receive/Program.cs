using System;
using System.IO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Util;
using System.Text;

namespace Receive
{
  class Receive
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

      factory.Ssl.ServerName = "master.sharenj.org"; //System.Net.Dns.GetHostName();
      factory.Ssl.CertPath = @"C:\code\api\rabbitmq-client-dotnet\Certificate\server.pem";
      factory.Ssl.Enabled = true;
      factory.Port = 5671;

      using(IConnection connection = factory.CreateConnection())
      using(IModel channel = connection.CreateModel())
      {
        channel.QueueDeclare(queue: "TestAppQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
          var body = ea.Body.ToArray();
          var message = Encoding.UTF8.GetString(body);
          Console.WriteLine(" [x] Received {0}", message);
        };
        channel.BasicConsume(queue: "TestAppQueue", autoAck: true, consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
      }
    }
  }
}
