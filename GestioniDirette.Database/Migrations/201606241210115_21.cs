namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _21 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notice", "Url", c => c.String());
            AddColumn("dbo.Notice", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notice", "Description");
            DropColumn("dbo.Notice", "Url");
        }
    }
}
