using Microsoft.AspNetCore.Identity;

namespace Deploying_Test.Models.Entities
{
    public class Owner : IdentityUser
    {
        public ICollection<Book> books { get; set; } = new List<Book>();
    }
}