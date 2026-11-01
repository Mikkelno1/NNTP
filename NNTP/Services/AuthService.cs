using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using NNTP.Models;

namespace NNTP.Services;

/**
 * Handles all authentication required by the server and encryption/storage of user informatino
 */
public class AuthService : IAuthService
{
    private readonly IConnection _connection;
    private const string AuthInfoUser = "authinfo user ";
    private const string AuthInfoPass = "authinfo pass ";
    
    
    
    public AuthService(IConnection connection)
    {
        _connection = connection;
    }

    /**
     * Sends username to server and awaits response
     */
    public async Task<bool> AuthenticateUserName(User user)
    {
        var sw = _connection.ReturnStreamWriter();
        var sr = _connection.ReturnStreamReader();
        var statusCode = "";
        
        var access = false;
        
        await sw.WriteLineAsync(AuthInfoUser + user.Username);
        await sw.FlushAsync();
        
        
        statusCode = await sr.ReadLineAsync();
        Console.WriteLine(user.Username + " this is the userbname inside AuthenticateUserName");
        Console.WriteLine(statusCode + " this is the statuscode inside AuthenticateUserName");

        if (string.IsNullOrEmpty(statusCode) || statusCode.Length < 3)
        {
            return access;
        }
        
        if (statusCode.Substring(0,3).Equals("381")) //only needs checking 381 as all other codes are invalid in this context
        {
            access = true;
        }

        return access;
    }

    /**
     * Sends password to server and awaits response
     */
    public async Task<bool> AuthenticatePassword(User user)
    {
        var sw = _connection.ReturnStreamWriter();
        var sr = _connection.ReturnStreamReader();
        var statusCode = "";
        
        var access = false;
        
        await sw.WriteLineAsync(AuthInfoPass + user.Password);
        await sw.FlushAsync();

        statusCode = await sr.ReadLineAsync();
        
        Console.WriteLine(user.Password + " this is the password inside AuthenticatePassword");
        Console.WriteLine(statusCode + " this is the statuscode inside AuthenticatePassword");

        if (string.IsNullOrEmpty(statusCode) || statusCode.Length < 3)
        {
            return access;
        }
        
        if (statusCode.Substring(0,3).Equals("281")) //only needs checking 281 as all other codes are invalid in this context
        {
            access = true;
        }

        return access;
    }

    /**
     * Encrypts username and password and saves to disc - yes, very secure
     * Borrowed from Microsoft documentation
     */
    public string EncryptInfo(string username, string password)
    {
        var statusMessage = "";
        
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

            statusMessage = "Info successfully encrypted";
        }
        catch (Exception e)
        {
            statusMessage = e.Message;
        }

        return statusMessage;
    }

    /**
     * Decrypts username and password and loads from disc - yes, very secure
     * Borrowed from Microsoft documentation
     */
    public List<string> DecryptInfo()
    {
        var statusMesasage = "";
        var username = "";
        var password = "";
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