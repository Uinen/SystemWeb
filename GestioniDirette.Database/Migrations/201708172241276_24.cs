namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _24 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PvErogatori", "Value", c => c.Int());
            AlterColumn("dbo.PvErogatori", "Value1", c => c.Int());
            AlterColumn("dbo.PvErogatori", "Value2", c => c.Int());
            AlterColumn("dbo.PvErogatori", "Value3", c => c.Int());
            AlterColumn("dbo.PvErogatori", "Value4", c => c.Int());
            AlterColumn("dbo.PvErogatori", "Value5", c => c.Int());
            AlterColumn("dbo.PvErogatori", "Value6", c => c.Int());
            AlterColumn("dbo.PvErogatori", "Value7", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PvErogatori", "Value7", c => c.Int(nullable: false));
            AlterColumn("dbo.PvErogatori", "Value6", c => c.Int(nullable: false));
            AlterColumn("dbo.PvErogatori", "Value5", c => c.Int(nullable: false));
            AlterColumn("dbo.PvErogatori", "Value4", c => c.Int(nullable: false));
            AlterColumn("dbo.PvErogatori", "Value3", c => c.Int(nullable: false));
            AlterColumn("dbo.PvErogatori", "Value2", c => c.Int(nullable: false));
            AlterColumn("dbo.PvErogatori", "Value1", c => c.Int(nullable: false));
            AlterColumn("dbo.PvErogatori", "Value", c => c.Int(nullable: false));
        }
    }
}
