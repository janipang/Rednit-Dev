namespace RednitDev.Models;

public class Comment
{
    public uint Id = 0;
    public User? User { get; set; }
    public string? Content { get; set; }
    public DateTime? Date {get; set;}

    public Comment? Reply {get; set;}
}
