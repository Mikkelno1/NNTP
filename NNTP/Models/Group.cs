namespace NNTP.Models;

public class Group
{
    public string numberOfArticles { get; set; }
    public string firstArticleNumber { get; set; }
    public string lastArticleNumber { get; set; }
    public string groupName { get; set; }
    
    
    public override string ToString()
    {
        return $"{numberOfArticles} {firstArticleNumber} {lastArticleNumber} {groupName}";
    }
}