using Microsoft.AspNetCore.Identity;

namespace Alps_Hiking.Entities;

public class User: IdentityUser
{
    public string? Fullname { get; set; }
    public List<Comment>? Comments { get; set; }
    public User()
    {
        Comments = new();
    }

}
