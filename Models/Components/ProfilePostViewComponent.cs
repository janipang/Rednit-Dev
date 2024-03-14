using System;
using Microsoft.AspNetCore.Mvc;
using RednitDev.Models;
using System.IO;
using System.Text.Json;
namespace RednitDev.Components
{
    public class ProfilePostViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int idPost)
        {
            // read posts file
            var postsJson = System.IO.File.ReadAllText("./Datacenter/post.json");
            Console.WriteLine("postsJson : " + postsJson);
            List<Post> posts;
            try
            {
                posts = JsonSerializer.Deserialize<List<Post>>(postsJson); //! is for not to warning me,jezz
            }
            catch (JsonException)
            {
                posts = new List<Post>();
            };
            Console.WriteLine("posts: " + posts.Count);
            var post = posts.SingleOrDefault(x => x.Id == idPost);
            Console.WriteLine("post: " + post);
            return View(post);
        }
        
    }

}
