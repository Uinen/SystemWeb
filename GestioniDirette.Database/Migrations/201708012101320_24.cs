namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _24 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PvErogatori", "Value1", c => c.Int(nullable: false));
            AddColumn("dbo.PvErogatori", "Value2", c => c.Int(nullable: false));
            AddColumn("dbo.PvErogatori", "Value3", c => c.Int(nullable: false));
            AddColumn("dbo.PvErogatori", "Value4", c => c.Int(nullable: false));
            AddColumn("dbo.PvErogatori", "Value5", c => c.Int(nullable: false));
            AddColumn("dbo.PvErogatori", "Value6", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PvErogatori", "Value6");
            DropColumn("dbo.PvErogatori", "Value5");
            DropColumn("dbo.PvErogatori", "Value4");
            DropColumn("dbo.PvErogatori", "Value3");
            DropColumn("dbo.PvErogatori", "Value2");
            DropColumn("dbo.PvErogatori", "Value1");
        }
    }
}
