using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext :IdentityDbContext<AppUser,AppRole,int,
                IdentityUserClaim<int>,AppUserRole,IdentityUserLogin<int>,
                IdentityRoleClaim<int>,IdentityUserToken<int>> //DbContext
    {//to use IdentityDbContext , install package
        public DataContext()
        {
        }

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        //public DbSet<AppUser> Users { get; set; } //remove it because IdentityDbContext provides it
        public DbSet<Photo> Photos { get; set; }
        public DbSet<userLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }  
        public DbSet<Group> Groups { get; set; }

        public DbSet<Connection> Connections { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            modelBuilder.Entity<AppRole>()
                .HasMany(ur=>ur.UserRoles) 
                .WithOne(u => u.Role).
                HasForeignKey(ur => ur.RoleId).
                IsRequired();




            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.Photos)
                .WithOne(p => p.User)
                .HasForeignKey(x=>x.AppUserId);

            modelBuilder.Entity<Photo>().HasKey(x => x.Id);


            modelBuilder.Entity<userLike>()
                .HasKey(k => new { k.SourceUserId, k.LikedUserId });

            modelBuilder.Entity<userLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUser)
                .HasForeignKey(s=>s.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction);//if delete user , delete the entities

            modelBuilder.Entity<userLike>()
                .HasOne(s => s.LikedUser)
                .WithMany(l => l.LikedByUser)
                .HasForeignKey(s => s.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade);//if delete user , delete the entities

            modelBuilder.Entity<Message>()
                .HasOne(u=>u.Recipient)
                .WithMany(m=>m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);//because we dont want to remove the messages
                                                   //if the other part didnt delete them selves

            modelBuilder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

        }


    }
}
