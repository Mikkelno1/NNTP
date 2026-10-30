using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NNTP.Models;

namespace NNTP.Services;

public class NewsGroupService : INewsGroupService
{
    private IConnection _connection;
    //private Article _articleObject;
    //private NewsGroup _ngs;
    //private Group _group;

    private List<NewsGroup> _articleList = new();
    private string _statusCodeNewsGroup;
    private string _statusCode;
    
    
    public NewsGroupService(IConnection connection)
    {
        _connection = connection;
    }
    

    public async Task<(string, Article)> FetchArticleAsync()
    {
        var sr = _connection.ReturnStreamReader();
        string delimiter = ": ";
        string line = await sr.ReadLineAsync();
        bool isMessage = false;
        StringBuilder sb = new();
        
        if (line == null)
        {
            _statusCode = "null";
        }
        else
        {
            _statusCode = line;
        } 

        Article article = new();


        if (_connection.CheckStatusCode(_statusCode))
        {
            while (true)
            {
                line = await sr.ReadLineAsync();

                if (line == null)
                {
                    break;
                }

                if (line.Equals("."))
                {
                    article.message = sb.ToString();
                    break;
                }

                if (string.IsNullOrEmpty(line))
                {
                    isMessage = true;
                    continue;
                }

                if (isMessage)
                {
                    sb.Append(line).AppendLine();
                    continue;
                }

                string[] word = line.Split(delimiter);


                switch (word[0])
                {
                    case "From":
                        article.fromPoster = line;
                        break;
                    case "Newsgroups":
                        article.newsGroup = line;
                        break;
                    case "Subject":
                        article.subject = line;
                        break;
                    case "Date":
                        article.date = line;
                        break;
                    case "Organization":
                        article.organization = line;
                        break;
                    case "Lines":
                        article.lines = line;
                        break;
                    case "Message-ID":
                        article.messageId = line;
                        break;
                    case "Injection-Date":
                        article.injectionDate = line;
                        break;
                    case "Injection-Info":
                        article.injectionInfo = line;
                        break;
                    case "User-Agent":
                        article.userAgent = line;
                        break;
                    case "Cancel-Lock":
                        article.cancelLock = line;
                        break;
                    case "Xref":
                        article.xref = line;
                        break;
                    default:
                        article.extraValues[word[0]] = line;
                        break;
                }
            }
        }

        return (_statusCode, article);
    }

    public async Task CallServerAsync(string command)
    {
        StreamWriter sw = _connection.ReturnStreamWriter();
        
        Console.WriteLine("This is the streamwriter inside callserver " + _connection.ReturnStreamWriter());

        if (_connection.IsConnected())
        {
            await sw.WriteLineAsync(command);
            Console.WriteLine(command + " this is the command");
        }
        else
        {
            Console.WriteLine("Not connected");
        }
        
        
    }

    
    
    
    public async Task<(string Statuscode, List<Group> groups)>FetchGroupArticleListAsync()
    {
        Console.WriteLine("This is the statuscode inside FetchGroupArticleListAsync " +_statusCode );
        
            return await GetListAsync(line =>
            {
                var words = line.Split(' ');
                return new Group
                {
                    numberOfArticles = words[0],
                    firstArticleNumber = words[1],
                    lastArticleNumber = words[2],
                    groupName = words[3],
                };
            });
        
    }
    
    public async Task<(string Statuscode, List<NewsGroup> newsGroups)> FetchNewsGroupListAsync()
    {
        
           return await GetListAsync(line =>
                   {
                       var words = line.Split(' ');
                       return new NewsGroup
                       {
                           name = words[0],
                           first = words[1],
                           last = words[2],
                           type = words[3]
                       };
                   }); 
        
    }
    
    public async Task<(string statusCode, Group group)> GetGroupList()
    {
        StreamReader sr = _connection.ReturnStreamReader();
        string line = await sr.ReadLineAsync();
        Console.WriteLine(line + " this is line after _sr.ReadLineAsync()");
        
        if (line == null)
        {
            Console.WriteLine(line + " this is line when line == null");
            return ("null", new Group());
        }

        Group group = new();
        string[] words = line.Split(' ');
        _statusCode = words[0];

        if (_connection.CheckStatusCode(_statusCode) && words.Length >= 5)
        {
            
            group.numberOfArticles = words[1];
            group.firstArticleNumber = words[2];
            group.lastArticleNumber = words[3];
            group.groupName = words[4];
            
        }
        else
        {
            group.numberOfArticles = "";
            group.firstArticleNumber = "";
            group.lastArticleNumber = "";
            group.groupName = "";
        }

        return (_statusCode, group);
    }
    
    public async Task<(string, List<T>)> GetListAsync<T>(Func<string, T> parse)
    {
        var list = new List<T>();
        var sr = _connection.ReturnStreamReader();
        string line = await sr.ReadLineAsync();
        
        

        if (line == null)
        {
            _statusCode = "null";
        }
        else
        {
            string[] words = line.Split(' ');
            _statusCode = words[0];
        }

        Console.WriteLine("This is the statuscode inside FetchNewsGroupListAsync " +_statusCode );
        
        
        if (_connection.CheckStatusCode(_statusCode))
        {
            while (true)
            {
                line = await sr.ReadLineAsync();
            
                if (line == null || line.Equals("."))
                {
                    break;
                }

                T item = parse(line);
            
                list.Add(item);
            }
        }
        
        return (_statusCode, list);
    }
    
}