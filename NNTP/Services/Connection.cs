using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using NNTP.Models;

namespace NNTP.Services;


public class Connection : IConnection
{
    public TcpClient client = new();
    
    public async Task Connect(Host host)
    {
        string address = host.Address;// en åben citat-server 
        int port = host.Port;
        

        // (2) (valgfrit) sæt korte timeouts
        client.ReceiveTimeout = 3000; 
        client.SendTimeout = 3000;// ikke så relevant til denne app!

            // (3) Forbind til serveren
        await client.ConnectAsync(address, port);
        
        // (7) Luk i omvendt rækkefølge
        //if (client != null) client.Close();
        
    }

    public Stream ReturnStream() => client.GetStream();
    
    
}
