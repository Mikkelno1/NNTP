using System.IO;
using System.Threading.Tasks;
using NNTP.Models;

namespace NNTP.Services;

public interface IConnection
{
     Task Connect(Host host);

     Stream ReturnStream();

     bool IsConnected();

     bool CheckStatusCode(string statusCode);

     StreamReader ReturnStreamReader();

     StreamWriter ReturnStreamWriter();
}
