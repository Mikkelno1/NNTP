using System.Collections.Generic;
using System.Linq;

namespace NNTP.Models;

public class Article
{
    public string articleId { get; set; }
    public string path { get; set; }
    public string fromPoster { get; set; }
    public string newsGroup { get; set; }
    public string subject { get; set; }
    public string date { get; set; }
    public string organization { get; set; }
    public string lines { get; set; }
    public string messageId { get; set; }
    public string injectionDate { get; set; }
    public string injectionInfo { get; set; }
    public string postingAccount { get; set; }
    public string userAgent { get; set; }
    public string cancelLock { get; set; }
    public string xref { get; set; }
    public string message { get; set; }

    public Dictionary<string, string> extraValues { get; set; } = new();
    
    public override string ToString()
    {
        return
            $"{nameof(articleId)}: {articleId}, {nameof(path)}: {path}, {nameof(fromPoster)}: {fromPoster}, " +
            $"{nameof(newsGroup)}: {newsGroup}, {nameof(subject)}: {subject}, {nameof(date)}: {date}, " +
            $"{nameof(organization)}: {organization}, {nameof(lines)}: {lines}, {nameof(messageId)}: {messageId}, " +
            $"{nameof(injectionDate)}: {injectionDate}, {nameof(injectionInfo)}: {injectionInfo}, " +
            $"{nameof(postingAccount)}: {postingAccount}, {nameof(userAgent)}: {userAgent}, {nameof(cancelLock)}: " +
            $"{cancelLock}, {nameof(xref)}: {xref}, {nameof(message)}: {message}, {nameof(extraValues)}: {extraValues.DictionaryString()}";
    }
}

public static class DictionaryExtensions
{
    public static string DictionaryString(this Dictionary<string, string> dictionary)
    {
        return "{" + string.Join(", ", dictionary.Select(kvp => $"{kvp.Key}: {kvp.Value}")) + "}";
    }
}