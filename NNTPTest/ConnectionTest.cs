using System.Net.Sockets;
using NNTP.Models;
using NNTP.Services;

namespace NNTPTest;

public class ConnectionTest
{
    [Fact]
    public void ChestStatusCode_AssertTrue()
    {
        var statusCode = "281";
        var connection = new Connection();

        bool result = connection.CheckStatusCode(statusCode);
        
        Assert.True(result);
    } 
    
    [Fact]
    public void ChestStatusCode_AssertFalse()
    {
        var statusCode = "381";
        var connection = new Connection();
        
        bool result = connection.CheckStatusCode(statusCode);
        
        Assert.False(result);
    }

    [Fact]
    public void ReturnStreamWriter_ObjectIsNullBeforeConnect()
    {
        var connection = new Connection();
        
        Assert.Null(connection.ReturnStreamWriter());
    }
    
    [Fact]
    public void ReturnStreamReader_ObjectIsNullBeforeConnect()
    {
        var connection = new Connection();
        
        Assert.Null(connection.ReturnStreamReader());
    }
    
    [Fact]
    public async Task ReturnStreamWriter_ObjectIsNotNullAfterConnect()
    {
        var connection = new Connection();
        var host = new Host("news.eternal-september.org", 119);

        await connection.Connect(host);
        
        Assert.NotNull(connection.ReturnStreamWriter());
    }
    
    [Fact]
    public async Task ReturnStreamReader_ObjectIsNotNullAfterConnect()
    {
        var connection = new Connection();
        var host = new Host("news.eternal-september.org", 119);

        await connection.Connect(host);
        
        Assert.NotNull(connection.ReturnStreamReader());
    }

    [Fact]
    public async Task Connect_WhenConnectionIsValid()
    {
        var connection = new Connection(); 
        var host = new Host("news.eternal-september.org", 119);

        await connection.Connect(host);
        bool result = connection.IsConnected();
        
        
        Assert.True(result);
    }
    
    [Fact]
    public async Task Connect_WhenConnectionIsInvalid()
    {
        var connection = new Connection(); 
        var host = new Host("someServer", 119);
        
        await Assert.ThrowsAsync<SocketException>(() => connection.Connect(host));
    }

    [Fact]
    public void SaveServer_MessageWhenFileCanBeSaved()
    {
        var connection = new Connection();
        var serverName = "news.eternal-september.org";
        var serverPort = "119";

        var expected = "Server successfully saved";
        var actual = connection.SaveServer(serverName, serverPort);
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void SaveServer_MessageWhenFileCannotBeSaved()
    {
        var connection = new Connection();
        var serverName = "news.eternal-september.org";
        var serverPort = "119";
        
        var actual = connection.SaveServer(serverName, serverPort);
        
        Assert.NotEmpty(actual);
    }
    
    [Fact]
    public void LoadServer_MessageWhenFileCannotBeSaved()
    {
        var connection = new Connection();
        
        var actual = connection.LoadServer();
        
        Assert.Single(actual);
    }
    
    [Fact]
    public void LoadServer_MessageWhenFileCanBeSaved()
    {
        var connection = new Connection();

        var statusMessage = "Server successfully loaded";
        var actual = connection.LoadServer();
        
        Assert.Contains(statusMessage, actual);
    }

    [Fact]
    public async Task WaitforSemaphore_SemaphoreLoadedWhenWaiting()
    {
        var connection = new Connection();
        
        await connection.WaitForSemaphore();
        Task wait = connection.WaitForSemaphore();
        Assert.False(wait.IsCompleted);
        connection.ReleaseSemaphore();
        await wait;

        Assert.True(wait.IsCompleted);
    }
    
    [Fact]
    public async Task WaitforSemaphore_SemaphoreNotLoadedBecauseFull()
    {
        var connection = new Connection();

        await connection.WaitForSemaphore();
        Task wait = connection.WaitForSemaphore();
        Assert.False(wait.IsCompleted);
        connection.ReleaseSemaphore();
        await wait;
        
        Assert.True(wait.IsCompleted);
    }

    [Fact]
    public async Task ReleaseSemaphore_CanRelease()
    {
        var connection = new Connection();
        
        await connection.WaitForSemaphore();
        Task wait = connection.WaitForSemaphore();
        connection.ReleaseSemaphore();
        await wait;
        
        Assert.True(wait.IsCompleted);
    }
    
    [Fact]
    public async Task WaitforSemaphore_SemaphoreNotReleased()
    {
        var connection = new Connection();

        await connection.WaitForSemaphore();
        Task wait = connection.WaitForSemaphore();
            
        Assert.False(wait.IsCompleted);
    }
}