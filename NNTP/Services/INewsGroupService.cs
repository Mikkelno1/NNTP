using System.Collections.Generic;
using System.Threading.Tasks;
using NNTP.Models;

namespace NNTP.Services;

public interface INewsGroupService
{
    Task CallServer(string command);

    Task ReadArticle();

    Task GetNewsGroupList();

    (string, List<NewsGroup>) ReturnNewsGroupList();

}