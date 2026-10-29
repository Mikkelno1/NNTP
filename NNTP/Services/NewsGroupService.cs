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
    private Article _articleObject;
    private NewsGroup _ngs;

    private StreamReader _sr = null;
    private StreamWriter _sw = null;

    private List<NewsGroup> _articleList = new();
    private string _statusCode;
    
    
    public NewsGroupService(IConnection connection, Article article, NewsGroup ngs)
    {
        _connection = connection;
        _articleObject = article;
        _ngs = ngs;
        _sw = new StreamWriter(_connection.ReturnStream());
        _sr = new StreamReader(_connection.ReturnStream(), Encoding.ASCII, detectEncodingFromByteOrderMarks: false, bufferSize: 1024);
    }

    public async Task ReadArticle()
    {
        string delimiter = ": ";
        string line;
        bool isMessage = false;
        StringBuilder sb = new();

        while (true)
        {
            line = await _sr.ReadLineAsync();
            
            if (line == null || line.Equals("."))
            {
                _articleObject.message = sb.ToString();
                return;
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
                    _articleObject.fromPoster = line;
                    break;
                case "Newsgroups" :
                    _articleObject.newsGroup = line;
                    break;
                case "Subject" :
                    _articleObject.subject = line;
                    break;
                case "Date" :
                    _articleObject.date = line;
                    break;
                case "organization" :
                    _articleObject.organization = line;
                    break;
                case "Lines" :
                    _articleObject.lines = line;
                    break;
                case "Message-ID" :
                    _articleObject.messageId = line;
                    break;
                case "Injection-Date" :
                    _articleObject.injectionDate = line;
                    break;
                case "Injection-Info" :
                    _articleObject.injectionInfo = line;
                    break;
                case "User-Agent" :
                    _articleObject.userAgent = line;
                    break;
                case "Cancel-Lock" :
                    _articleObject.cancelLock = line;
                    break;
                case "Xref" :
                    _articleObject.xref = line;
                    break;
                default:
                    _articleObject.extraValues[word[0]] = line.Substring(word[0].Length + 2);
                    break;
            }
        }
    }

    public async Task CallServer(string command)
    {
        _sw.AutoFlush = true;
        
        await _sw.WriteLineAsync(command);
    }

    public async Task GetNewsGroupList()
    {
        _articleList.Clear();
        
        _sr = new StreamReader(_connection.ReturnStream(), Encoding.ASCII, detectEncodingFromByteOrderMarks: false, bufferSize: 1024);
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
            
            _ngs = new();
            
            string[] word = line.Split(delimiter);

            _ngs.name = word[0];
            _ngs.first = word[1];
            _ngs.last = word[2];
            _ngs.type = word[3];
            
            _articleList.Add(_ngs);
        }
    }

    public (string, List<NewsGroup>) ReturnNewsGroupList() => (_statusCode, _articleList);
}