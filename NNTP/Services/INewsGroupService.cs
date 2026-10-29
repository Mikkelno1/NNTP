using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NNTP.Models;

namespace NNTP.Services;

public interface INewsGroupService
{
    Task CallServerAsync(string command);

    Task<Article> FetchArticleAsync();

    //Task<(string Statuscode, List<NewsGroup> groups)> FetchGroupAsync();

    //Task<(string Statuscode, List<Group> groups)> FetchGroupArticleListAsync();

    //Task<(string, List<T>)> GetListAsync<T>(Func<string, T> parse);

    public Task<(string, List<Group>)> GetGroupList();

    Task<(string, List<NewsGroup>)> GetNewsGroupList();

}