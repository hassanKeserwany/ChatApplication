using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace API.Entities
{
    public class AppRole : IdentityRole<int>
    {
        //each appUser can has many roles
        //each role can contain many users (many to many)

        [JsonIgnore] // This prevents circular reference during serialization

        public ICollection<AppUserRole> UserRoles { get; set;}

    }
}
