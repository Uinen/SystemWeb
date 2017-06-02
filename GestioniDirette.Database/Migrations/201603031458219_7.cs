namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _7 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Year", "year", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Year", "year", c => c.Int(nullable: false));
        }
    }
}
