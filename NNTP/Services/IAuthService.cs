using System.Collections.Generic;
using System.Threading.Tasks;
using NNTP.Models;

namespace NNTP.Services;

public interface IAuthService
{
    Task<bool> AuthenticateUserName(User user);

    Task<bool> AuthenticatePassword(User user);

    string EncryptInfo(string username, string password);

    List<string> DecryptInfo();
}