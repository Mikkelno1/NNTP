using System.IO;
using System.Threading.Tasks;
using NNTP.Models;

namespace NNTP.Services;

public interface IConnection
{
     public Task Connect(Host host);

     public Stream ReturnStream();
}