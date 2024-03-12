namespace RednitDev.Models;
public class Noti
{
    public string? Type {get; set; } // success fail request
    public int IdPost {get; set;}
    public Account? WhoRequest {get; set;}
}