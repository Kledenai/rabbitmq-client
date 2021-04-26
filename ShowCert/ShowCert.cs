using System;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.IO;
using RabbitMQ.Client;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ShowCert
{
  class ShowCert
  {
    static void Main(string[] args)
    {
      try
      {
          var x509 = new X509Certificate2(File.ReadAllBytes(@"C:\certs\rabbitmq\client.cer"));
          //Create X509Certificate2 object from .cer file.

          //Print to console information contained in the certificate.
          Console.WriteLine("{0}Subject: {1}{0}", Environment.NewLine, x509.Subject);
          Console.WriteLine("{0}Issuer: {1}{0}", Environment.NewLine, x509.Issuer);
          Console.WriteLine("{0}Version: {1}{0}", Environment.NewLine, x509.Version);
          Console.WriteLine("{0}Valid Date: {1}{0}", Environment.NewLine, x509.NotBefore);
          Console.WriteLine("{0}Expiry Date: {1}{0}", Environment.NewLine, x509.NotAfter);
          Console.WriteLine("{0}Thumbprint: {1}{0}", Environment.NewLine, x509.Thumbprint);
          Console.WriteLine("{0}Serial Number: {1}{0}", Environment.NewLine, x509.SerialNumber);
          Console.WriteLine("{0}Friendly Name: {1}{0}", Environment.NewLine, x509.PublicKey.Oid.FriendlyName);
          Console.WriteLine("{0}Public Key Format: {1}{0}", Environment.NewLine, x509.PublicKey.EncodedKeyValue.Format(true));
          Console.WriteLine("{0}Raw Data Length: {1}{0}", Environment.NewLine, x509.RawData.Length);
          Console.WriteLine("{0}Certificate to string: {1}{0}", Environment.NewLine, x509.ToString(true));
          Console.WriteLine("{0}Certificate to XML String: {1}{0}", Environment.NewLine, x509.PublicKey.Key.ToXmlString(false));
      }
      catch (DirectoryNotFoundException)
      {
              Console.WriteLine("Error: The directory specified could not be found.");
      }
      catch (IOException)
      {
          Console.WriteLine("Error: A file in the directory could not be accessed.");
      }
      catch (NullReferenceException)
      {
          Console.WriteLine("File must be a .cer file. Program does not have access to that type of file.");
      }
    }
  }
}
