using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NNTP.Models;

namespace NNTP.Services;

/**
 * Handles connection to the server and storing/loading of server data
 */
public class Connection : IConnection
{
    public TcpClient client;
    private StreamWriter _sw;
    private StreamReader _sr;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    
    /**
     * Handles connection to server via host object
     * Borrowed from Tommy
     */
    public async Task Connect(Host host)
    {
        string address = host.Address;
        int port = host.Port;
        
        client = new();
        
        client.ReceiveTimeout = 3000; 
        client.SendTimeout = 3000;
        
        await client.ConnectAsync(address, port);
        
        _sr = new StreamReader(ReturnStream(), Encoding.ASCII, detectEncodingFromByteOrderMarks: false, 1024, leaveOpen:true);
        
        _sw = new StreamWriter(ReturnStream(), Encoding.ASCII, leaveOpen:true);
        _sw.AutoFlush = true;

        string initialMessage = await _sr.ReadLineAsync();
        Console.WriteLine(initialMessage + " this is the server welcome message");

    }

    /**
     * Returns the stream to ensure same stream is used across the entire program
     */
    public Stream ReturnStream() => client.GetStream();

    /**
     * Checks if still connected to the server
     */
    public bool IsConnected()
    {
        return client != null && client.Connected;
    }

    /**
     * Checks if status code is either of the "2" or "3" family - used to gate certain behaviour in other parts of the service layer
     */
    public bool CheckStatusCode(string statusCode)
    {
        if (string.IsNullOrEmpty(statusCode))
        {
            return false;
        }
        if (statusCode[0] == '2')
        {
            return true;
        } 
        
        if (statusCode[0] == '3')
        {
            return true;
        }
        
        return false;
    }
    
    /**
     * Exposes the streamreader from this service to other services to ensure same stream is used
     */
    public StreamReader ReturnStreamReader()
    {
        return _sr;
    }

    /**
     * Exposes the streamwriter from this service to other services to ensure same stream is used
     */
    public StreamWriter ReturnStreamWriter()
    {
        return _sw;
    }

    /**
     * Saves the server and port to a plaintext document on the harddrive for easy loading and loging
     */
    public string SaveServer(string serverName, string serverPort)
    {
        string statusMesasage = "";

        try
        {
            using (FileStream fileStream = new(@"C:\Users\Bruger\Desktop\ServerInfo.txt", FileMode.OpenOrCreate))
            {
                using (StreamWriter saverWriter = new(fileStream))
                {
                    saverWriter.WriteLine(serverName);
                    saverWriter.WriteLine(serverPort);
                }
            }

            statusMesasage = "Server successfully saved";
        }
        catch (Exception e)
        {
            statusMesasage = e.Message;
        }

        return statusMesasage;
    }

    /**
     * Loads the server and port to a plaintext document on the harddrive for easy loading and loging
     */
    public List<string> LoadServer()
    {
        string statusMesasage = "";
        string serverName = "";
        string serverPort = "";
        List<string> list = new();
        
        try
        {
            using (FileStream fileStream = new(@"C:\Users\Bruger\Desktop\ServerInfo.txt", FileMode.Open))
            {
                using (StreamReader loadWriter = new(fileStream))
                {
                    serverName = loadWriter.ReadLine();
                    list.Add(serverName);
                            
                    serverPort = loadWriter.ReadLine();
                    list.Add(serverPort);
                }
                    
            }
            statusMesasage = "Server successfully loaded";
        }
        catch (Exception e)
        {
            statusMesasage = e.Message;
            
        }
        
        list.Add(statusMesasage);

        return list;
    }

    /**
     * Exposes semaphore.WaitAsync() for usage in other classes across the service layer
     */
    public async Task WaitForSemaphore()
    {
        await _semaphore.WaitAsync();
    }

    /**
     * Exposes semaphore.Release() for usage in other classes across the service layer
     */
    public void ReleaseSemaphore()
    {
        _semaphore.Release();
    }
}
