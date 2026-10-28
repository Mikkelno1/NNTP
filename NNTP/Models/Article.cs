using System.Collections.Generic;

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

    public Dictionary<string, string> extraValues { get; set; }
    
}