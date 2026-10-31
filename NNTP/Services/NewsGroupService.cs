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

    private const string postCommand = "post";
    
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
        string statuscode = "";
        
        if (line == null)
        {
            statuscode = "null";
        }
        else
        {
            statuscode = line;
        } 

        Article article = new();


        if (_connection.CheckStatusCode(statuscode))
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

        return (statuscode, article);
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

    
    
    /*
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
    */
    
    
    public async Task<(string Statuscode, List<NewsGroup> newsGroups)> FetchNewsGroupListAsync()
    {
        
           return await GetListAsync(line =>
                   {
                       var words = line.Split(' ');
                       return new NewsGroup
                       {
                           name = words[0],
                           firstArticleNo = words[1],
                           lastArticleNo = words[2],
                           postingStatus = words[3]
                       };
                   }); 
    }
    
    public async Task<(string statusCode, Group group)> GetGroupList()
    {
        StreamReader sr = _connection.ReturnStreamReader();
        string line = await sr.ReadLineAsync();
        Console.WriteLine(line + " this is line after _sr.ReadLineAsync()");
        string statuscode = "";
        
        if (line == null)
        {
            Console.WriteLine(line + " this is line when line == null");
            return ("null", new Group());
        }

        Group group = new();
        string[] words = line.Split(' ');
        statuscode = words[0];

        if (_connection.CheckStatusCode(statuscode) && words.Length >= 5)
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

        return (statuscode, group);
    }
    
    
    public async Task<(string, List<T>)> GetListAsync<T>(Func<string, T> parse)
    {
        var list = new List<T>();
        var sr = _connection.ReturnStreamReader();
        string line = await sr.ReadLineAsync();
        string statuscode = "";
        

        if (line == null)
        {
            statuscode = "null";
        }
        else
        {
            string[] words = line.Split(' ');
            statuscode = words[0];
        }

        Console.WriteLine("This is the statuscode inside FetchNewsGroupListAsync " +statuscode );
        
        
        if (_connection.CheckStatusCode(statuscode))
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
        
        return (statuscode, list);
    }
    

    public async Task<string> SendCreatedArticle(Article article)
    {
        StreamWriter sw = _connection.ReturnStreamWriter();
        StreamReader sr = _connection.ReturnStreamReader();

        string statuscode = "";

        try
        {
            await sw.WriteLineAsync(postCommand);
            
            await sw.FlushAsync();
            
            statuscode = await sr.ReadLineAsync();

            Console.WriteLine(_connection.IsConnected() + " this is the server connection status");

            if (statuscode.Substring(0,3).Equals("340"))
            {
                await sw.WriteAsync(
                    "From: <" + article.fromPoster + ">\r\n" +
                    "Newsgroups: " + article.newsGroup + "\r\n" +
                    "Subject: " + article.subject + "\r\n" +
                    "\r\n" +
                    article.message + "\r\n" +
                    ".\r\n");

                await sw.FlushAsync();
            }
            
            statuscode = await sr.ReadLineAsync();
            Console.WriteLine(statuscode + " this is the statuscode inside FetchCreatedArticle after sending");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return statuscode;
    }
    
}