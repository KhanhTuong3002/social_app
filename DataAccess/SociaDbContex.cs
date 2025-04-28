
using BussinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccess
{
    public class SociaDbContex : DbContext
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Post>()
                .Property(p => p.UserId)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.UserId)
                .HasMaxLength(20)
                .IsRequired();

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

            base.OnModelCreating(modelBuilder);

        }
    }
}
