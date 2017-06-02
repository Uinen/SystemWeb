namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Year", "yearNumber", c => c.Int(nullable: false));
            CreateIndex("dbo.Year", "yearNumber");
            DropColumn("dbo.Year", "year");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Year", "year", c => c.Long(nullable: false));
            DropIndex("dbo.Year", new[] { "yearNumber" });
            DropColumn("dbo.Year", "yearNumber");
        }
    }
}
