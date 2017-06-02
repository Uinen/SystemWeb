namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Year", "year", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Year", "year");
        }
    }
}
