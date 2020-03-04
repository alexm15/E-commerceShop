namespace Webshop.UI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TableContrainsProductAndCategory : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Categories", "Name", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Products", "Name", c => c.String(nullable: false, maxLength: 1000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "Name", c => c.String());
            AlterColumn("dbo.Categories", "Name", c => c.String());
        }
    }
}
