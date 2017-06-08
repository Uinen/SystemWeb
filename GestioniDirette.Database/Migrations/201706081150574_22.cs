namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _22 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Licenza", "Scadenza", c => c.DateTime());
            AddColumn("dbo.Dispenser", "Scadenza", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dispenser", "Scadenza");
            DropColumn("dbo.Licenza", "Scadenza");
        }
    }
}
