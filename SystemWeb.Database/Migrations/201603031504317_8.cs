namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _8 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Year", "year");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Year", "year", c => c.Long(nullable: false));
        }
    }
}
