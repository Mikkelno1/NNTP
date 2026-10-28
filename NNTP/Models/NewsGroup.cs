namespace NNTP.Models;

public class NewsGroup
{
    public string name { get; set; }
    public string last { get; set; }
    public string first { get; set; }
    public string type { get; set; }

    public override string ToString()
    {
        return $"{name} {last} {first} {type}";
    }
}