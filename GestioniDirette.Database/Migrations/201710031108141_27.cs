namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _27 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reclami",
                c => new
                    {
                        ReclamiID = c.Guid(nullable: false),
                        pvID = c.Guid(nullable: false),
                        Tipologia = c.Int(nullable: false),
                        Reclamante = c.String(),
                        Documento = c.Int(nullable: false),
                        NumeroDocumento = c.String(),
                        Cellulare = c.String(),
                        DataEvento = c.DateTime(nullable: false),
                        ImportoInserito = c.Int(nullable: false),
                        ImportoMancato = c.Double(nullable: false),
                        ImportoRimanente = c.Double(nullable: false),
                        NumeroInterno = c.Int(nullable: false),
                        NumeroAssegnato = c.String(),
                        Note = c.String(),
                        DataCreazione = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReclamiID)
                .ForeignKey("dbo.Pv", t => t.pvID, cascadeDelete: true)
                .Index(t => t.pvID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reclami", "pvID", "dbo.Pv");
            DropIndex("dbo.Reclami", new[] { "pvID" });
            DropTable("dbo.Reclami");
        }
    }
}
