namespace NNTP.Models;

public class NewsGroup
{
    public string name { get; set; }
    public string lastArticleNo { get; set; }
    public string firstArticleNo { get; set; }
    public string postingStatus { get; set; }

    public override string ToString()
    {
        return $"{name} {lastArticleNo} {firstArticleNo} {postingStatus}";
    }
}