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
                    article.extraValues[word[0]] = line.Substring(word[0].Length + 2);
                    break;
            }
        }
        
        return article;
    }

    public async Task CallServerAsync(string command)
    {
        _sw.AutoFlush = true;
        
        await _sw.WriteLineAsync(command);
    }

    
    
    
    /*
    
    
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
    */
    
    public async Task<(string, List<Group>)> GetGroupList()
    {
        string line = await _sr.ReadLineAsync();
        
        
        char delimiter = ' ';
        
        List<Group> groupList = new();
        
        
        if (line == null ||line.Equals("."))
        {
            return (line, groupList);
        }
            
        string[] word = line.Split(delimiter);

        if (word.Length >= 5)
        {
            Group group = new Group();
            
            _statusCode = word[0];
            group.numberOfArticles = word[1];
            group.firstArticleNumber = word[2];
            group.lastArticleNumber = word[3];
            group.groupName = word[4];
            
            groupList.Add(group);
        }

        return (_statusCode, groupList);
    }
    
    public async Task<(string, List<NewsGroup>)> GetNewsGroupList()
    {
        string line;
        char delimiter = ' ';
        _statusCodeNewsGroup = await _sr.ReadLineAsync();

        List<NewsGroup> newsGroupList = new();
        
        while (true)
        {
            line = await _sr.ReadLineAsync();
            
            if (line == null || line.Equals("."))
            {
                break;
            }
            
            
            string[] word = line.Split(delimiter);

            if (word.Length >= 4)
            {
                NewsGroup ngs = new();
                ngs.name = word[0];
                ngs.first = word[1];
                ngs.last = word[2];
                ngs.type = word[3];
                
                newsGroupList.Add(ngs);
            }
        }

        return (_statusCodeNewsGroup, newsGroupList);
    }
    
    
    
    /*
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

    /*
    public async Task GetNewsGroupList()
    {
        _articleList.Clear();
        
        string line;
        char delimiter = ' ';
        _statusCodeNewsGroup = await _sr.ReadLineAsync();
        
        
        while (true)
        {
            line = await _sr.ReadLineAsync();
            
            if (line == null ||line.Equals("."))
            {
                return;
            }
            
            _ngs = new();
            
            string[] word = line.Split(delimiter);

            _ngs.name = word[0];
            _ngs.first = word[1];
            _ngs.last = word[2];
            _ngs.type = word[3];
            
            _articleList.Add(_ngs);
        }
    }

    public (string, List<NewsGroup>) ReturnNewsGroupList() => (_statusCodeNewsGroup, _articleList);
    
    public async Task GetGroupList()
    {
        _groupList.Clear();
        
        string line;
        char delimiter = ' ';
        _statusCode = await _sr.ReadLineAsync();
        
        while (true)
        {
            line = await _sr.ReadLineAsync();
            
            if (line == null ||line.Equals("."))
            {
                return;
            }
            
            _group = new();
            
            string[] word = line.Split(delimiter);

            _group.numberOfArticles = word[0];
            _group.firstArticleNumber = word[1];
            _group.lastArticleNumber = word[2];
            _group.groupName = word[3];
            
            _groupList.Add(_group);
        }
    }

    public async Task<(string, List<T>)> GetList<T>(Func<string, T> parse)
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
    
    public (string, List<Group>) ReturnGroupList() => (_statusCode, _groupList);
    */
}