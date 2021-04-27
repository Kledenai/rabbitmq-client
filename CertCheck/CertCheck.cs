/* This is a Mono tool, for checking if an SSLv3
 * certificate can be used for Server validation
 * during an SSLv3 handshake
 *
 * Compile with:
 *  gmcs -r:Mono.Security certcheck.cs
 *
 * Usage:
 *  mono certcheck.exe /path/to/acertfile.cer
 * */

using System;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;

using Mono.Security.X509;
using Mono.Security.X509.Extensions;


class CertCheck
{
    public static void CheckStep(string msg)
    {
        Console.Write(msg + "... ");
    }

    public static void CheckOk()
    {
        Console.WriteLine("Ok");
    }

    public static void CheckFail(string msg)
    {
        Console.WriteLine(msg);
        ExitInvalid();
    }

    public static void ExitInvalid()
    {
        Console.WriteLine("This certificate CAN NOT be used by Mono for Server validation");
        Environment.Exit(0);
    }

    public static void ExitValid()
    {
        Console.WriteLine("This certificate CAN be used by Mono for Server validation");
    }

    public static void Main(string[] args)
    {
        if(args.Length < 1) {
            Console.WriteLine("Usage: certcheck <certfile.cer>");
            return;
        }

        byte[] bytes = File.ReadAllBytes(args[0]);
        X509Certificate cert = new X509Certificate(bytes);


        CheckStep("Checking if certificate is SSLv3");
        if(cert.Version != 3) {
            CheckFail("No");
        }
        CheckOk();


        CheckStep("Checking for KeyUsageExtension (2.5.29.15)");

        X509Extension xtn = cert.Extensions["2.5.29.15"];
        if(xtn == null) {
            CheckFail("Not present");
        }
        CheckOk();


        CheckStep("Checking if KeyEncipherment flag is set");
        KeyUsageExtension kux = new KeyUsageExtension(xtn);
        if(!kux.Support(KeyUsages.keyEncipherment)) {
            CheckFail("Not set");
        }

        CheckOk();
        ExitValid();
    }
}
