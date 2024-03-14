namespace RednitDev.Models;

public class User
{
    public int Id { get; set; }
    public Account Account { get; set; }
    public Profile Profile { get; set; }
    public List<Noti> Noti { get; set; }

}