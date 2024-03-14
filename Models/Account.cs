namespace RednitDev.Models;

public class Account
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password {get; set;}

    public override bool Equals(object? obj)
    {
        Account account = obj as Account;
        if(account == null) return false;
        return Username == account.Username &&
        Email == account.Email &&
        Password == account.Password;
    }
}