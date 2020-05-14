namespace E_commerce.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductVariant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ParentId", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "ParentId");
        }
    }
}
