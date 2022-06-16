using Microsoft.AspNetCore.Identity;

namespace APIDOP.Models.DB
{
    public class User: IdentityUser<Guid>
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public ICollection<Topic> Topics { get; set; }
    }
}
