namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _23 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PvCali",
                c => new
                    {
                        PvCaliId = c.Guid(nullable: false),
                        PvTankId = c.Guid(nullable: false),
                        Value = c.Int(nullable: false),
                        FieldDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PvCaliId)
                .ForeignKey("dbo.PvTank", t => t.PvTankId, cascadeDelete: true)
                .Index(t => t.PvTankId);
            
            CreateTable(
                "dbo.PvDeficienze",
                c => new
                    {
                        PvDefId = c.Guid(nullable: false),
                        PvTankId = c.Guid(nullable: false),
                        Value = c.Int(nullable: false),
                        FieldDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PvDefId)
                .ForeignKey("dbo.PvTank", t => t.PvTankId, cascadeDelete: true)
                .Index(t => t.PvTankId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PvDeficienze", "PvTankId", "dbo.PvTank");
            DropForeignKey("dbo.PvCali", "PvTankId", "dbo.PvTank");
            DropIndex("dbo.PvDeficienze", new[] { "PvTankId" });
            DropIndex("dbo.PvCali", new[] { "PvTankId" });
            DropTable("dbo.PvDeficienze");
            DropTable("dbo.PvCali");
        }
    }
}
