using System.Data.Entity;
using Webshop.UI.Models;

namespace Webshop.UI.App_Data
{
    public class WebshopContext : DbContext
    {
        public WebshopContext()
            : base("name=WebshopContext")
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(e => e.Products)
                .WithMany(e => e.Categories)
                .Map(m => m.ToTable("CategoryEntries").MapLeftKey("CategoryId").MapRightKey("ProductId"));

            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .HasMaxLength(1000)
                .IsRequired();
        }
    }
}
