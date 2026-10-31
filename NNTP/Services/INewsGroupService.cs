using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NNTP.Models;

namespace NNTP.Services;

public interface INewsGroupService
{
    Task CallServerAsync(string command);

    Task<(string, Article)> FetchArticleAsync();
    
    //Task<(string Statuscode, List<Group> groups)> FetchGroupArticleListAsync();

    Task<(string, List<T>)> GetListAsync<T>(Func<string, T> parse);

    Task<(string Statuscode, List<NewsGroup> newsGroups)> FetchNewsGroupListAsync();

    Task<(string statusCode, Group group)> GetGroupList();

    Task<string> SendCreatedArticle(Article article);


}