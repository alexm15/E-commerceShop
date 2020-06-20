using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using E_commerce.Library;

namespace E_commerce.Data
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

            modelBuilder.Entity<Product>()
                .HasOptional(p => p.ParentProduct)
                .WithMany(p => p.Variants)
                .HasForeignKey(p => p.ParentId);
        }
    }
}
