namespace RednitDev.Models;

public class Profile
{
    public string? Caption {get; set;} = "";
    public List<int> CreatedPosts {get; set;} = [];
    public List<int> InterestedPosts {get; set;} = [];
    public List<int> JoinningPosts {get; set;} = [];
    public List<int> RequesingPosts {get; set;} = [];
    public string? Image {get; set;} = "";
    public List<string> InterestedTag {get; set;} = [];
}