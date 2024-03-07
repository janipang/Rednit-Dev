namespace RednitDev.Models;

public class User
{
    Account account = new Account();
    Profile profile = new Profile();
    public string? Username { get; set; }

}