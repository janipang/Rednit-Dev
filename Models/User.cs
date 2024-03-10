namespace RednitDev.Models;

public class User
{
    public Account? Account { get; set; }
    public Profile? Profile { get; set; }

    public Account AccountSetter   // property
    {
      get { return Account!; }
      set { Account = value; }
    }
    public Profile ProfileSetter   // property
    {
      get { return Profile!; }
      set { Profile = value; }
    }
}