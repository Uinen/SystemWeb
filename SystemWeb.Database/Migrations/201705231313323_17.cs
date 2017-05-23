namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _17 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Documento", "FileName");
            DropColumn("dbo.Documento", "Tipo");
            DropColumn("dbo.Documento", "UploadDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Documento", "UploadDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Documento", "Tipo", c => c.Int(nullable: false));
            AddColumn("dbo.Documento", "FileName", c => c.String());
        }
    }
}
