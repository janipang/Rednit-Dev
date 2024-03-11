namespace RednitDev.Models;
public class Post
{
    public User? Author { get; set; }
    public PostDetail? Detail { get; set; }
    public EventDate? EventDate { get; set; }
    public bool Requesting { get; set; }
    public bool Visible { get; set; }
    public int MemberCount { get; set; } = 0;
    public int MemberMax { get; set; }
    public int? DayLeft { get; set; } //pls help me to calc this
    public List<Comment> Comments { get; set; } = [];
    public List<Account> Liked { get; set; }  = []; //User that liked post
    public List<Account> Joined { get; set; }  = []; //When User click join (open), //When User request has accepted(request)
    public List<Account>?Requested { get; set; }  = []; //User that request to join
}