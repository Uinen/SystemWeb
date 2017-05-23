namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _5 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Month", new[] { "OneLetterMonth" });
            DropColumn("dbo.Month", "OneLetterMonth");
        }
        
        public override void Down()
        {
        }
    }
}
