using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NNTP.Models;

namespace NNTP.Services;

/**
 * Handles all request/response of data from the server related to list (Newsgroups), group (Group) and article (Article) NNTP commands 
 */
public class NewsGroupService : INewsGroupService
{
    private IConnection _connection;

    private const string postCommand = "post";
    
    public NewsGroupService(IConnection connection)
    {
        _connection = connection;
    }
    

    /**
     * Fetches Article using the article NNTP command and handles saving data to related Article object
     */
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

                if (line.Equals(".")) //checks if the message has ended
                {
                    article.message = sb.ToString();
                    break;
                }

                if (string.IsNullOrEmpty(line)) //checks if the method has finished appending all headers to related article header attributes
                {
                    isMessage = true;
                    continue;
                }

                if (isMessage)
                {
                    sb.Append(line).AppendLine(); //if all headers are assigned their attributes, the rest of the article is saved to the message attribute
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

    /**
     * Sends NNTP commands to server
     */
    public async Task CallServerAsync(string command)
    {
        StreamWriter sw = _connection.ReturnStreamWriter();
        
        Console.WriteLine("This is the streamwriter inside callserver " + _connection.ReturnStreamWriter());

        if (_connection.IsConnected()) //checks connection before sending command
        {
            await sw.WriteLineAsync(command);
            Console.WriteLine(command + " this is the command");
        }
        else
        {
            Console.WriteLine("Not connected");
        }
    }
    
    /**
     * Exposes the GetListAsync method to the viewmodel
     * Leftover from initial idea about keeping code DRY - see GetListAsync
     */
    public async Task<(string Statuscode, List<NewsGroup> newsGroups)> FetchNewsGroupListAsync()
    {
           return await GetListAsync(line =>
                   {
                       var words = line.Split(' ');
                       return new NewsGroup
                       {
                           name = words[0],
                           lastArticleNo = words[1],
                           firstArticleNo= words[2],
                           postingStatus = words[3]
                       };
                   }); 
    }
    
    /**
     * Handles assigning fetched groups to their group attributes
     */
    public async Task<(string statusCode, Group group)> GetGroup()
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

        if (_connection.CheckStatusCode(statuscode) && words.Length >= 5) //makes sure that the group structure remain uniform
        {
            group.numberOfArticles = words[1];
            group.firstArticleNumber = words[2];
            group.lastArticleNumber = words[3];
            group.groupName = words[4];
        }
        else
        {
            group = null;
        }

        return (statuscode, group);
    }
    
    
    
    /**
     * Handles reading the newsgroup list
     * Was initially trying to create one syntactical similar method to handle groups and articles, but realized after creating this
     * that the methods would be too varied. Kept this method and its exposing method instead of keeping the structure similar across the service
     */
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
    

    /**
     * Sends article made in the articleviewmodel to the server
     */
    public async Task<string> SendCreatedArticle(Article article)
    {
        string statuscode = "";

        try
        {
            StreamWriter sw = _connection.ReturnStreamWriter();
            StreamReader sr = _connection.ReturnStreamReader();

            Console.WriteLine("This is inside SendCreatedArticle before sending POST");
            await sw.WriteLineAsync(postCommand);

            statuscode = await sr.ReadLineAsync(); //empties buffer

            Console.WriteLine(statuscode + "This is the STATUSCODE inside SendCreatedArticle after sending POST");

            if (string.IsNullOrEmpty(statuscode) || statuscode.Length < 3)
            {
                return statuscode;
            }

            if (statuscode.Substring(0, 3).Equals("340"))
            {
                await sw.WriteAsync(
                    "From: " + article.fromPoster + "\r\n" +
                    "Newsgroups: " + article.newsGroup + "\r\n" +
                    "Subject: " + article.subject + "\r\n\r\n" +
                    article.message +
                    "\r\n.\r\n");

                
            }
            await sw.FlushAsync();
            
            statuscode = await sr.ReadLineAsync(); //empties buffer 
            Console.WriteLine(statuscode + " this is the statuscode inside FetchCreatedArticle after sending");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        return statuscode;
    }
    
    public async Task<string> ReadServerResponse()
    {
        return await _connection.ReturnStreamReader().ReadLineAsync();
    }
}