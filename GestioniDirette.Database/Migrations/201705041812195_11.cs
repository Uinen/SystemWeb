namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _11 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Cartissima", "sCartPhone", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Cartissima", "sCartPhone", c => c.Int(nullable: false));
        }
    }
}
