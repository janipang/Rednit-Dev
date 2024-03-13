namespace RednitDev.Models;

public class User
{
    public Account Account { get; set; }
    public Profile Profile { get; set; }
    public List<Noti> Noti { get; set; }
    public int Id { get; set; }
}