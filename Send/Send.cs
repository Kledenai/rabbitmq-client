using System;
using System.IO;
using System.Text;
using RabbitMQ.Client;
using System.Net.Security;
using RabbitMQ.Client.Exceptions;
using System.Security.Cryptography.X509Certificates;

namespace Send
{
  class Send
  {
    public static X509Certificate SelectLocalCertificate(
      object sender,
      string targetHost,
      X509CertificateCollection localCertificates,
      X509Certificate remoteCertificate,
      string[] acceptableIssuers)
    {
      Console.WriteLine("Client is selecting a local certificate.");
      if (acceptableIssuers != null &&
        acceptableIssuers.Length > 0 &&
        localCertificates != null &&
        localCertificates.Count > 0)
      {
        // Use the first certificate that is from an acceptable issuer.
        foreach (X509Certificate certificate in localCertificates)
        {
          string issuer = certificate.Issuer;
          if (Array.IndexOf(acceptableIssuers, issuer) != -1)
              return certificate;
        }
      }
      if (localCertificates != null &&
        localCertificates.Count > 0)
        return localCertificates[0];

      return null;
    }

    public static void Main()
    {
      try
      {
        var factory = new ConnectionFactory()
        {
            HostName = "master.sharenj.org",
            VirtualHost = "jkalil",
            //UserName = "CN=kirk.sharenj.org,OU=Ops,O=org,DC=org",
            //Password = "guest",
            Port = 5671,
        };

        factory.AuthMechanisms =  new IAuthMechanismFactory[]{ new ExternalMechanismFactory()};

        factory.Ssl.Version = System.Security.Authentication.SslProtocols.Tls13;

        factory.Ssl.AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNotAvailable |
                                              SslPolicyErrors.RemoteCertificateChainErrors |
                                              SslPolicyErrors.RemoteCertificateNameMismatch;

        // var x509 = new X509Certificate2(File.ReadAllBytes(@"C:/code/api/rabbitmq-client/Certificate/client.cer"));

        // Console.WriteLine(x509.Subject);

        // factory.Ssl.CertificateSelectionCallback = new LocalCertificateSelectionCallback(SelectLocalCertificate);

        factory.Ssl.ServerName = "master.sharenj.org";
        //factory.Ssl.CertPath = @"C:\code\api\rabbitmq-client\Certificate\server.pem";
        factory.Ssl.Enabled = true;
        using (IConnection connection = factory.CreateConnection())
        {
          using (IModel channel = connection.CreateModel())
          {
            channel.QueueDeclare(queue: "TestAppQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            string message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "", routingKey: "TestAppQueue", basicProperties: properties, body: body);

            BasicGetResult result = channel.BasicGet("TestAppQueue", true);
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
