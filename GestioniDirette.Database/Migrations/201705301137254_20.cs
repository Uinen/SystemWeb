namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Carico", new[] { "documentoId" });
            RenameColumn(table: "dbo.Carico", name: "Deposito", newName: "depId");
            RenameIndex(table: "dbo.Carico", name: "IX_Deposito", newName: "IX_depId");
            CreateIndex("dbo.Carico", "DocumentoID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Carico", new[] { "DocumentoID" });
            RenameIndex(table: "dbo.Carico", name: "IX_depId", newName: "IX_Deposito");
            RenameColumn(table: "dbo.Carico", name: "depId", newName: "Deposito");
            CreateIndex("dbo.Carico", "documentoId");
        }
    }
}
