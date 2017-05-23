namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _6 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Year", new[] { "year" });
        }
        
        public override void Down()
        {
           
        }
    }
}
