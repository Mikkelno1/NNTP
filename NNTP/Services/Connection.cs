using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using NNTP.Models;

namespace NNTP.Services;


public class Connection : IConnection
{
    public void Connect(Host host)
    {
        string address = host.Address;// en åben citat-server 
        int port = host.Port;

        TcpClient client = null;
        NetworkStream ns = null;
        StreamReader reader = null;

        try
        {
            // (1) Opret klient
            client = new TcpClient();

            // (2) (valgfrit) sæt korte timeouts
            client.ReceiveTimeout = 3000;
            client.SendTimeout = 3000;// ikke så relevant til denne app!

            // (3) Forbind til serveren
            client.Connect(address, port);
            

            if (client.Connected)
            {
                Console.WriteLine("Jeg er connected");
            }
            
               
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fejl: " + ex.Message);
        }
        finally
        {
            // (7) Luk i omvendt rækkefølge
            if (reader != null) reader.Close();
            if (ns != null) ns.Close();
            if (client != null) client.Close();
        }

        Console.Read();
    }
}
