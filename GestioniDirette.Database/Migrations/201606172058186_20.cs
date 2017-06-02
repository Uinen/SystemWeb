namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PvTankDesc",
                c => new
                    {
                        PvTankDescId = c.Guid(nullable: false),
                        PvTankId = c.Guid(nullable: false),
                        PvTankCM = c.Single(nullable: false),
                        PvTankLT = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.PvTankDescId)
                .ForeignKey("dbo.PvTank", t => t.PvTankId, cascadeDelete: true)
                .Index(t => t.PvTankId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PvTankDesc", "PvTankId", "dbo.PvTank");
            DropIndex("dbo.PvTankDesc", new[] { "PvTankId" });
            DropTable("dbo.PvTankDesc");
        }
    }
}
