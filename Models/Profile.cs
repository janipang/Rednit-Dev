namespace RednitDev.Models;

public class Profile
{
    public string? Caption {get; set;}
    public List<Post>? CreatedPosts {get; set;}
    public List<Post>? InterestedPosts {get; set;}
    public List<Post>? JoinningPosts {get; set;}
    public string? Image {get; set;}
    public List<string> InterestedTag {get; set;} = [];
    public List<string>? Contact {get; set;}
}