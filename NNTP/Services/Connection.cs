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
    public TcpClient client;
    private StreamWriter _sw;
    private StreamReader _sr;
    
    public async Task Connect(Host host)
    {
        string address = host.Address;// en åben citat-server 
        int port = host.Port;
        
        client = new();
        

        // (2) (valgfrit) sæt korte timeouts
        client.ReceiveTimeout = 3000; 
        client.SendTimeout = 3000;// ikke så relevant til denne app!

            // (3) Forbind til serveren
        await client.ConnectAsync(address, port);
        
        
        _sr = new StreamReader(ReturnStream(), Encoding.ASCII, detectEncodingFromByteOrderMarks: false, 1024, leaveOpen:true);
        
        _sw = new StreamWriter(ReturnStream(), leaveOpen:true);
        _sw.AutoFlush = true;

        string initialMessage = await _sr.ReadLineAsync();
        Console.WriteLine(initialMessage + " this is the server welcome message");
        // (7) Luk i omvendt rækkefølge
        //if (client != null) client.Close();

    }

    public Stream ReturnStream() => client.GetStream();

    public bool IsConnected()
    {
        return client != null && client.Connected;
    }

    public bool CheckStatusCode(string statusCode)
    {
        if (statusCode[0] == '2')
        {
            return true;
        }

        return false;
    }
    
    public StreamReader ReturnStreamReader()
    {
        return _sr;
    }

    public StreamWriter ReturnStreamWriter()
    {
        return _sw;
    }
}
