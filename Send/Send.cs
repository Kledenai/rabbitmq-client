using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Send
{
  class Send
  {
    public static void Main()
    {
      try
      {
        var factory = new ConnectionFactory()
        {
            HostName = "master.sharenj.org",
            VirtualHost = "jkalil",
            Port = 5671,
        };

        factory.AuthMechanisms = new IAuthMechanismFactory[]{ new ExternalMechanismFactory()};

        factory.Ssl.Version = System.Security.Authentication.SslProtocols.Tls12;

        factory.Ssl.ServerName = "kirk.sharenj.org";
        factory.Ssl.CertPath = @"C:\code\api\rabbitmq-client\Certificate\client.pem";
        factory.Ssl.Enabled = true;
        using (IConnection connection = factory.CreateConnection())
        {
          using (IModel channel = connection.CreateModel())
          {
            channel.QueueDeclare(queue: "TestAppQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            string message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "TestAppQueue", basicProperties: null, body: body);

            BasicGetResult result = channel.BasicGet("rabbitmq-dotnet-test", true);
              if (result == null)
              {
                  Console.WriteLine("No message received.");
              }
              else
              {
                Console.WriteLine("Received:", message);
                // DebugUtil.DumpProperties(result, Console.Out, 0);
              }
          }
        }
      }
      catch (BrokerUnreachableException bex)
      {
          Exception ex = bex;
          while (ex != null)
          {
              Console.WriteLine(ex.Message);
              Console.WriteLine("inner:");
              ex = ex.InnerException;
          }
      }
      catch (Exception ex)
      {
          Console.WriteLine(ex.ToString());
      }
      Console.ReadKey();
    }
  }
}
