using System;
using RabbitMQ.Client;
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
        Port = 5671,
      };

      factory.Ssl.Version = System.Security.Authentication.SslProtocols.Tls12;

      factory.Ssl.ServerName = "master.sharenj.org";
      factory.Ssl.CertPath = @"C:\code\api\rabbitmq-client-dotnet\Certificate\server.pem";
      factory.Ssl.Enabled = true;

      using(IConnection connection = factory.CreateConnection())
      using(IModel channel = connection.CreateModel())
      {
        channel.QueueDeclare(queue: "TestAppQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

        string message = "Hello World! 2";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "", routingKey: "TestAppQueue", basicProperties: null, body: body);

        Console.WriteLine(" [x] Sent {0}", message);
      }

      Console.WriteLine(" Press [enter] to exit.");
      Console.ReadLine();
    }
  }
}
