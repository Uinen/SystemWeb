namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _9 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cartissima",
                c => new
                    {
                        sCartId = c.Guid(nullable: false),
                        pvID = c.Guid(nullable: false),
                        sCartCreateDate = c.DateTime(nullable: false),
                        sCartIp = c.String(),
                        sCartName = c.String(),
                        sCartSurname = c.String(),
                        sCartEmail = c.String(),
                        sCartPhone = c.Int(nullable: false),
                        sCartCompany = c.String(),
                        sCartIva = c.Int(nullable: false),
                        sCartLocation = c.String(),
                        sCartVeichle = c.Int(nullable: false),
                        sCartVeichleType = c.String(),
                        sCartProcessed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.sCartId)
                .ForeignKey("dbo.Pv", t => t.pvID, cascadeDelete: true)
                .Index(t => t.pvID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cartissima", "pvID", "dbo.Pv");
            DropIndex("dbo.Cartissima", new[] { "pvID" });
            DropTable("dbo.Cartissima");
        }
    }
}
