using System.Threading.Tasks;
using NNTP.Models;

namespace NNTP.Services;

public interface IAuthService
{
    Task<bool> AuthenticateUserName(User user);

    Task<bool> AuthenticatePassword(User user);
}