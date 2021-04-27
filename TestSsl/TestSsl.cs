using System;
using System.IO;
using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Util;

namespace TestSsl
{
  public class TestSsl
  {
    public static int Main(string[] args)
    {
      ConnectionFactory cf = new ConnectionFactory();

      cf.HostName = "master.sharenj.org";
      cf.Ssl.Enabled = true;
      cf.Ssl.ServerName = "master.sharenj.org";
      cf.Ssl.CertPath = @"C:\code\api\rabbitmq-client\Certificate\client.cer";

      using (IConnection conn = cf.CreateConnection()) {
        using (IModel ch = conn.CreateModel()) {
        Console.WriteLine("Successfully connected and opened a channel");
        ch.QueueDeclare("rabbitmq-dotnet-test", false, false, false, null);
        Console.WriteLine("Successfully declared a queue");
        // ch.QueueDelete("rabbitmq-dotnet-test");
        // Console.WriteLine("Successfully deleted the queue");
        }
      }
      return 0;
    }
  }
}
