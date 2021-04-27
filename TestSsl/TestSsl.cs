using System;
using System.IO;
using System.Text;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using RabbitMQ.Util;

namespace TestSsl
{
  public class TestSsl
  {
    public static void Main()
    {
      try
      {
        ConnectionFactory cf = new ConnectionFactory();

        cf.Ssl.Enabled = true;
        cf.Ssl.ServerName = System.Net.Dns.GetHostName();
        cf.Ssl.CertPath = @"C:\code\api\rabbitmq-client\Certificate\client.p12";
        cf.Ssl.CertPassphrase = "test";

        cf.HostName = "master.sharenj.org";

        using (IConnection conn = cf.CreateConnection()) {
          using (IModel ch = conn.CreateModel()) {
          Console.WriteLine("Successfully connected and opened a channel");
          ch.QueueDeclare("rabbitmq-dotnet-test", false, false, false, null);
          Console.WriteLine("Successfully declared a queue");
          // ch.QueueDelete("rabbitmq-dotnet-test");
          // Console.WriteLine("Successfully deleted the queue");
          }
        }
        Console.ReadKey();
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
