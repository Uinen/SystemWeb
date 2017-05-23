namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _15 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PvTank", "Rimanenza", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PvTank", "Rimanenza");
        }
    }
}
