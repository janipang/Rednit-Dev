namespace RednitDev.Models;

public class Profile
{
    public string? Caption {get; set;} = "";
    public List<int> CreatedPosts {get; set;} = [];
    public List<int> InterestedPosts {get; set;} = [];
    public List<int> JoinningPosts {get; set;} = [];

    public List<int> RequesingPosts {get; set;} = [];

    public string? Image {get; set;} = "https://i0.wp.com/www.stignatius.co.uk/wp-content/uploads/2020/10/default-user-icon.jpg?fit=415%2C415&ssl=1";
    public List<string> InterestedTag {get; set;} = [];
}