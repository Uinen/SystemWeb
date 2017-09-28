namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _26 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Carico", "Lube", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Carico", "Lube", c => c.Int(nullable: false));
        }
    }
}
