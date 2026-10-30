using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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

    
    public string EncryptInfo(string username, string password)
    {
        string statusMesasage = "";
        
        try
        {

            using (FileStream fileStream = new(@"C:\Users\Bruger\Desktop\UserInfo.txt", FileMode.OpenOrCreate))
            {
                using (Aes aes = Aes.Create())
                {
                    byte[] key =
                    {
                        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                        0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
                    };
                    aes.Key = key;

                    byte[] iv = aes.IV;
                    fileStream.Write(iv, 0, iv.Length);

                    using (CryptoStream encryptionStream = new(fileStream, aes.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                    {
                        using (StreamWriter encryptWriter = new(encryptionStream))
                        {
                            encryptWriter.WriteLine(username);
                            encryptWriter.WriteLine(password);
                        }
                    }
                }
            }

            statusMesasage = "Info successfully encrypted";
        }
        catch (Exception e)
        {
            statusMesasage = e.Message;
        }

        return statusMesasage;
    }

    
    public List<string> DecryptInfo()
    {
        string statusMesasage = "";
        string username = "";
        string password = "";
        List<string> list = new();
        
        try
        {

            using (FileStream fileStream = new(@"C:\Users\Bruger\Desktop\UserInfo.txt", FileMode.Open))
            {
                using (Aes aes = Aes.Create())
                {
                    byte[] iv = new byte[aes.IV.Length];
                    fileStream.Read(iv, 0, iv.Length);
                    
                    
                    byte[] key =
                    {
                        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                        0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
                    };
                    
                    using (CryptoStream decryptionStream = new(fileStream, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                    {
                        using (StreamReader decryptReader = new(decryptionStream))
                        {
                            username = decryptReader.ReadLine();
                            list.Add(username);
                            
                            password = decryptReader.ReadLine();
                            list.Add(password);
                        }
                    }
                }
            }

            statusMesasage = "Info successfully decrypted";
        }
        catch (Exception e)
        {
            statusMesasage = e.Message;
        }
        
        list.Add(statusMesasage);

        return list;
    }
}