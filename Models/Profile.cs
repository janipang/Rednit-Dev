namespace RednitDev.Models;

public class Profile
{
    public List<Post>? CreatedPosts {get; set;}
    public List<Post>? InterestedPosts {get; set;}
    public List<Post>? JoinningPosts {get; set;}
    public string? Image {get; set;}
    public List<string>? Tag {get; set;}
    public List<string>? Contact {get; set;}
    public string? Noti {get; set;}


}