using System;
using System.Text;
using RabbitMQ.Client;
using System.Net.Security;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Receive
{
  class Receive
  {
    public static void Main()
    {
      try
      {
        var factory = new ConnectionFactory() {
          HostName = "master.sharenj.org",
          VirtualHost = "jkalil",
        };

        factory.AuthMechanisms = new IAuthMechanismFactory[]{ new ExternalMechanismFactory()};

        factory.Ssl.Version = System.Security.Authentication.SslProtocols.Tls12;

        factory.Ssl.AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNotAvailable |
                                              SslPolicyErrors.RemoteCertificateChainErrors |
                                              SslPolicyErrors.RemoteCertificateNameMismatch;

        factory.Ssl.ServerName = "master.sharenj.org";
        factory.Ssl.CertPath = @"C:\code\api\rabbitmq-client\Certificate\client.cer";
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
