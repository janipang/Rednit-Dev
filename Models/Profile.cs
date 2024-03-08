namespace RednitDev.Models;

public class Profile
{
    public string? Image { get; set; }

    public List<Post> Posts {get; set;} = [];
    public List<Post> InterestedPosts {get; set;} = [];
    public List<string> InterestedTag {get; set;} = [];

}