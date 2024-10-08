﻿using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace API.Entities
{
    public class AppUser:IdentityUser<int>
    {
        /*public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }*/
        //we dont need these properties because already found in IdentityUser
        public DateTime DateOfBirth { get; set; } 
        public string KnownAs { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string Lookingfor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<userLike> LikedByUser { get; set; }
        public ICollection<userLike> LikedUser { get; set; }
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesReceived { get; set; }

        [JsonIgnore] // This prevents circular reference during serialization

        public ICollection<AppUserRole> UserRoles { get; set; }



    }
}
