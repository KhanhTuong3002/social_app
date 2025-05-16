
using BussinessObject.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccess
{
    public class SociaDbContex : IdentityDbContext<User, IdentityRole<string>,string>
    {
        public SociaDbContex() { }
        public SociaDbContex(DbContextOptions<SociaDbContex> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }


        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<HashTag> HashTags { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<FriendShip> FriendShips { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Message> Message { get; set; } // Rename to Message

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Post>()
                .Property(p => p.UserId)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<User>().
                HasMany(u => u.Stories)
                .WithOne(p => p.User)
                .HasForeignKey(u => u.UserId);

            modelBuilder.Entity<User>()
               .HasMany(u => u.HashTags)
               .WithOne(h => h.User)
               .HasForeignKey(h => h.UserId)
               .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of a User if HashTags exist

            modelBuilder.Entity<Post>()
                .HasMany(p => p.HashTags)
                .WithOne(h => h.Post)
                .HasForeignKey(h => h.PostId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Like>().
                HasKey(l => new { l.UserId, l.PostId });

            modelBuilder.Entity<Like>().
                HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Like>().
                HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //comment
            modelBuilder.Entity<Comment>().
               HasOne(l => l.Post)
               .WithMany(p => p.Comments)
               .HasForeignKey(l => l.PostId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>().
                HasOne(l => l.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            //favorite
            modelBuilder.Entity<Favorite>().
                HasKey(l => new { l.UserId, l.PostId });

            modelBuilder.Entity<Favorite>().
                HasOne(l => l.Post)
                .WithMany(p => p.Favorites)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Favorite>().
                HasOne(l => l.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //Report
            modelBuilder.Entity<Report>().
                HasKey(l => new { l.UserId, l.PostId });

            modelBuilder.Entity<Report>().
                HasOne(l => l.Post)
                .WithMany(p => p.Reports)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Report>().
                HasOne(l => l.User)
                .WithMany(u => u.Reports)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            //message
            modelBuilder.Entity<Message>()
               .HasOne(m => m.Sender)
               .WithMany(u => u.MessagesSent)
               .HasForeignKey(m => m.SenderId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.MessagesReceived)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);


            //customize the ASP.Net Identity model table
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<string>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

            // Configure the FriendRequest entity

            modelBuilder.Entity<FriendRequest>()
               .HasOne(fr => fr.sender)
               .WithMany()
               .HasForeignKey(fr => fr.SenderId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.receiver)
                .WithMany()
                .HasForeignKey(fr => fr.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FriendShip>()
                .HasOne(fs => fs.Sender)
                .WithMany()
                .HasForeignKey(fs => fs.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FriendShip>()
                .HasOne(fs => fs.Receiver)
                .WithMany()
                .HasForeignKey(fs => fs.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
