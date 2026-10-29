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

    private StreamReader _sr = null;
    private StreamWriter _sw = null;

    private List<NewsGroup> _articleList = new();
    private string _statusCodeNewsGroup;
    private string _statusCode;
    
    
    public NewsGroupService(IConnection connection)
    {
        _connection = connection;
        _sw = new StreamWriter(_connection.ReturnStream());
        _sr = new StreamReader(_connection.ReturnStream(), Encoding.ASCII, detectEncodingFromByteOrderMarks: false, bufferSize: 1024);
    }

    public async Task<Article> FetchArticleAsync()
    {
        string delimiter = ": ";
        string line = await _sr.ReadLineAsync();;
        bool isMessage = false;
        StringBuilder sb = new();
        string statusCode;
        
        if (line == null)
        {
            statusCode = "null";
        }
        else
        {
            statusCode = line;
        }

        Article article = new();
        
        while (true)
        {
            line = await _sr.ReadLineAsync();

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
                case "From" :
                    article.fromPoster = line;
                    break;
                case "Newsgroups" :
                    article.newsGroup = line;
                    break;
                case "Subject" :
                    article.subject = line;
                    break;
                case "Date" :
                    article.date = line;
                    break;
                case "Organization" :
                    article.organization = line;
                    break;
                case "Lines" :
                    article.lines = line;
                    break;
                case "Message-ID" :
                    article.messageId = line;
                    break;
                case "Injection-Date" :
                    article.injectionDate = line;
                    break;
                case "Injection-Info" :
                    article.injectionInfo = line;
                    break;
                case "User-Agent" :
                    article.userAgent = line;
                    break;
                case "Cancel-Lock" :
                    article.cancelLock = line;
                    break;
                case "Xref" :
                    article.xref = line;
                    break;
                default:
                    article.extraValues[word[0]] = line;
                    break;
            }
        }
        
        return article;
    }

    public async Task CallServerAsync(string command)
    {
        _sw.AutoFlush = true;
        
        await _sw.WriteLineAsync(command);
        
        Console.WriteLine(command + " this is the command");
    }

    
    
    
    public async Task<(string Statuscode, List<Group> groups)>FetchGroupArticleListAsync()
    {
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
        string line = await _sr.ReadLineAsync();
        
        if (line == null)
        {
            return ("null", new Group());
        }

        string[] words = line.Split(' ');
        _statusCode = words[0];

        Group group = new Group
        {
            numberOfArticles = words[1],
            firstArticleNumber = words[2],
            lastArticleNumber = words[3],
            groupName = words[4]
        };

        return (_statusCode, group);
    }
    
    public async Task<(string, List<T>)> GetListAsync<T>(Func<string, T> parse)
    {
        var list = new List<T>();
        string line = await _sr.ReadLineAsync();
        string statusCode;

        if (line == null)
        {
            statusCode = "null";
        }
        else
        {
            statusCode = line;
        }
        
        while (true)
        {
            line = await _sr.ReadLineAsync();
            
            if (line == null || line.Equals("."))
            {
                break;
            }

            T item = parse(line);
            
            list.Add(item);
        }

        return (statusCode, list);
    }
}