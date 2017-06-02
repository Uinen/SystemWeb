namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _10 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Cartissima", "sCartName", c => c.String(nullable: false));
            AlterColumn("dbo.Cartissima", "sCartSurname", c => c.String(nullable: false));
            AlterColumn("dbo.Cartissima", "sCartCompany", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Cartissima", "sCartCompany", c => c.String());
            AlterColumn("dbo.Cartissima", "sCartSurname", c => c.String());
            AlterColumn("dbo.Cartissima", "sCartName", c => c.String());
        }
    }
}
