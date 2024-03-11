
using Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Database
{
    public class OtherContext : DbContext
    {
        public OtherContext(DbContextOptions options) : base(options)
        {
        }
    }

    public class Context : IdentityDbContext<User, IdentityRole, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<Image> Resources { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public Context(DbContextOptions options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            
            
            //Constraints
            mb.Entity<Group>().Property(g => g.Name)
                .HasMaxLength(Values.Entity.GroupNameMaxLength);
            mb.Entity<Group>().Property(g => g.Description)
                .HasMaxLength(Values.Entity.GroupDescriptionMaxLength);


            mb.Entity<Post>().Property(p => p.Content)
                .HasColumnType("TEXT");
            mb.Entity<Post>().Property(p => p.Summary)
                .HasMaxLength(Values.Entity.PostSummaryMaxLength);
            mb.Entity<Post>().Property(p => p.Title)
                .HasMaxLength(Values.Entity.PostTitleMaxLength);

            mb.Entity<Tag>().Property(t => t.Name)
                .HasMaxLength(Values.Entity.TagNameMaxLength);
            mb.Entity<Tag>().HasKey(t => t.Name);
                


            mb.Entity<User>().Property(t => t.PresentationName)
                .HasMaxLength(Values.Entity.UserPresentationNameMaxLength);

            mb.Entity<PostTag>().Property(pt => pt.TagName)
                .HasMaxLength(Values.Entity.TagNameMaxLength);
            mb.Entity<PostTag>().HasKey(pt => new { pt.TagName, pt.PostId });

            mb.Entity<Image>().Property(r => r.Path)
                .HasMaxLength(Values.Entity.ResourcePathMaxLength);

            mb.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId, ur.GroupId });

            
            
            
            //Relações
            mb.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.Owner)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<User>()
                .HasMany(u => u.Images)
                .WithOne(r => r.Owner)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<Post>()
                .HasOne(p => p.FeaturedImage)
                .WithMany(r => r.UsedByPosts)
                .OnDelete(DeleteBehavior.Restrict);
                
            mb.Entity<Post>()
                .HasMany(p => p.PostTags)
                .WithOne(pt => pt.Post)
                .OnDelete(DeleteBehavior.Cascade); ;

            mb.Entity<Tag>()
                .HasMany(t => t.PostTags)
                .WithOne(pt => pt.Tag)
                .OnDelete(DeleteBehavior.Cascade); ;
            mb.Entity<Group>()
                .HasOne(g => g.Image)
                .WithMany(im => im.UserByGroups)
                .OnDelete(DeleteBehavior.Restrict);
                
                



        }
    }
}
