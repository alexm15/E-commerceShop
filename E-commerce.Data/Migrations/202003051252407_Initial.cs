using System.Data.Entity.Migrations;

namespace E_commerce.Data.Migrations
{
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Products",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    Description = c.String(),
                    Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.CategoryEntries",
                c => new
                {
                    CategoryId = c.Int(nullable: false),
                    ProductId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.CategoryId, t.ProductId })
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.CategoryId)
                .Index(t => t.ProductId);

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CategoryEntries", "ProductId", "dbo.Products");
            DropForeignKey("dbo.CategoryEntries", "CategoryId", "dbo.Categories");
            DropIndex("dbo.CategoryEntries", new[] { "ProductId" });
            DropIndex("dbo.CategoryEntries", new[] { "CategoryId" });
            DropTable("dbo.CategoryEntries");
            DropTable("dbo.Products");
            DropTable("dbo.Categories");
        }
    }
}
