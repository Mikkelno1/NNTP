using NNTP.Models;

namespace NNTP.Services;

public interface IConnection
{
     public void Connect(Host host);
}