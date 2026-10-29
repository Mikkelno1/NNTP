using System;
using System.IO;
using System.Threading.Tasks;
using NNTP.Models;

namespace NNTP.Services;

public class AuthService : IAuthService
{
    private IConnection _connection;
    private const string authInfoUser = "authinfo user ";
    private const string authInfoPass = "authinfo pass ";
    private string _statusCode;
    private StreamWriter _sw;
    private StreamReader _sr;
    
    
    public AuthService(IConnection connection)
    {
        _connection = connection;
    }

    public async Task<bool> AuthenticateUserName(User user)
    {
        _sw = _connection.ReturnStreamWriter();
        _sr = _connection.ReturnStreamReader();
        
        var access = false;
        
        await _sw.WriteLineAsync(authInfoUser + user.Username);
        await _sw.FlushAsync();
        
        _statusCode = await _sr.ReadLineAsync();
        Console.WriteLine(user.Username + " this is the userbname inside AuthenticateUserName");
        Console.WriteLine(_statusCode + " this is the statuscode inside AuthenticateUserName");

        if (_statusCode.Substring(0,3).Equals("381"))
        {
            access = true;
        }

        return access;
    }

    public async Task<bool> AuthenticatePassword(User user)
    {
        _sw = _connection.ReturnStreamWriter();
        _sr = _connection.ReturnStreamReader();
        
        var access = false;
        
        await _sw.WriteLineAsync(authInfoPass + user.Password);
        await _sw.FlushAsync();

        _statusCode = await _sr.ReadLineAsync();
        
        Console.WriteLine(user.Password + " this is the password inside AuthenticatePassword");
        Console.WriteLine(_statusCode + " this is the statuscode inside AuthenticatePassword");

        if (_statusCode.Substring(0,3).Equals("281"))
        {
            access = true;
        }

        return access;
    }
}