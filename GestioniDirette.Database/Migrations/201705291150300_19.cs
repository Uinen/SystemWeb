namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _19 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Carico", "Documento");
            DropColumn("dbo.Carico", "Emittente");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Carico", "Emittente", c => c.String(maxLength: 32));
            AddColumn("dbo.Carico", "Documento", c => c.String(maxLength: 4));
        }
    }
}
