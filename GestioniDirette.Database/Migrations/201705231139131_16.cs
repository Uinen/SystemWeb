namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _16 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Deposito",
                c => new
                    {
                        depId = c.Guid(nullable: false),
                        Nome = c.String(),
                    })
                .PrimaryKey(t => t.depId);
            
            CreateTable(
                "dbo.Documento",
                c => new
                    {
                        DocumentoID = c.Guid(nullable: false),
                        FileName = c.String(),
                        Tipo = c.Int(nullable: false),
                        UploadDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DocumentoID);
            
            AddColumn("dbo.Carico", "documentoId", c => c.Guid(nullable: true));
            AddColumn("dbo.Carico", "Deposito", c => c.Guid(nullable: true));
            CreateIndex("dbo.Carico", "documentoId");
            CreateIndex("dbo.Carico", "Deposito");
            AddForeignKey("dbo.Carico", "Deposito", "dbo.Deposito", "depId", cascadeDelete: true);
            AddForeignKey("dbo.Carico", "documentoId", "dbo.Documento", "DocumentoID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Carico", "documentoId", "dbo.Documento");
            DropForeignKey("dbo.Carico", "Deposito", "dbo.Deposito");
            DropIndex("dbo.Carico", new[] { "Deposito" });
            DropIndex("dbo.Carico", new[] { "documentoId" });
            DropColumn("dbo.Carico", "Deposito");
            DropColumn("dbo.Carico", "documentoId");
            DropTable("dbo.Documento");
            DropTable("dbo.Deposito");
        }
    }
}
