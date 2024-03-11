namespace RednitDev.Models;

public class Comment
{
    public User? User { get; set; }
    public string? Content { get; set; }
    public DateTime? Date {get; set;}

    public Comment? Reply {get; set;}
}