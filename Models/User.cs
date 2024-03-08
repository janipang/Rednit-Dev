namespace RednitDev.Models;

public class User
{
    Account Account { get; set; }
    Profile Profile { get; set; }

    public Account AccountSetter   // property
  {
    get { return Account; }
    set { Account = value; }
  }

}