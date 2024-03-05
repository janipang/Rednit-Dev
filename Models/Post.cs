namespace RednitDev.Models;
public class Post
{
    public string? Header { get; set; }
    public string? Tag { get; set; }
    public string? Intro { get; set; }
    public string? Detail { get; set; }
    public string? Place { get; set; }
    public bool? request { get; set; }
    public int? memberCount { get; set; }
    public int? dayLeft { get; set; }
}