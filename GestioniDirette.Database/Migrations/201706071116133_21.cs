namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _21 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Licenza",
                c => new
                    {
                        LicenzaID = c.Guid(nullable: false),
                        pvID = c.Guid(nullable: false),
                        Codice = c.String(),
                        nPrecedente = c.String(),
                        nSuccessivo = c.String(),
                    })
                .PrimaryKey(t => t.LicenzaID)
                .ForeignKey("dbo.Pv", t => t.pvID, cascadeDelete: true)
                .Index(t => t.pvID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Licenza", "pvID", "dbo.Pv");
            DropIndex("dbo.Licenza", new[] { "pvID" });
            DropTable("dbo.Licenza");
        }
    }
}
