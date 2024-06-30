using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<userLike> UserLikes { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

        }


    }
}
