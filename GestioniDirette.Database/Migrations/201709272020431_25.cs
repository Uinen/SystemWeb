namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _25 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carico", "Lube", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Carico", "Lube");
        }
    }
}
