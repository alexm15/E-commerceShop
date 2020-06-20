namespace E_commerce.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductVariants : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ParentId", c => c.Int(nullable: true));
            CreateIndex("dbo.Products", "ParentId");
            AddForeignKey("dbo.Products", "ParentId", "dbo.Products", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "ParentId", "dbo.Products");
            DropIndex("dbo.Products", new[] { "ParentId" });
            DropColumn("dbo.Products", "ParentId");
        }
    }
}
