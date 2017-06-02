namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _22 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Notice", "Url");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notice", "Url", c => c.String());
        }
    }
}
