namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3 : DbMigration
    {
        public override void Up()
        {
            //AlterColumn("dbo.UsersImage", "UploadDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            //AlterColumn("dbo.UsersImage", "UploadDate", c => c.DateTime(nullable: false));
        }
    }
}
