using Deploying_Test.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Deploying_Test.Data
{
    public class ApplicationDbContext : IdentityDbContext<Owner>
    {
        public DbSet<Book> Books { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
                

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Book>()
                .HasOne(b => b.Owner)
                .WithMany(o => o.books)
                .HasForeignKey(b => b.OwnerId)
                .IsRequired();

        }






    }
}
