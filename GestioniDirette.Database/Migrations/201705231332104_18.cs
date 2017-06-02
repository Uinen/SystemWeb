namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _18 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documento", "Tipo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Documento", "Tipo");
        }
    }
}
