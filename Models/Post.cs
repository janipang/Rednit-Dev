namespace RednitDev.Models;
public class Post
{
    public User? Author { get; set; }
    public PostDetail? Detail { get; set; }
    public EventDate? EventDate { get; set; }
    public bool Requesting { get; set; }
    public bool Visible { get; set; }
    public int MemberCount { get; set; }
    public int MemberMax { get; set; }
    public int? DayLeft { get; set; }
    public List<Comment>? Comments { get; set; }
    public List<User>? Liked { get; set; } //User that liked post
    public List<User>? Joined { get; set; } //When user click join (open), //When user request has accepted(request)
    public List<User>? Requested { get; set; } //User that request to join
}