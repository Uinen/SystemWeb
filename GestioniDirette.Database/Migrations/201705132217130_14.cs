namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _14 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Carico", "yearId", "dbo.Year");
            DropIndex("dbo.Carico", new[] { "yearId" });
            AlterColumn("dbo.Carico", "yearId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Carico", "yearId");
            AddForeignKey("dbo.Carico", "yearId", "dbo.Year", "yearId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Carico", "yearId", "dbo.Year");
            DropIndex("dbo.Carico", new[] { "yearId" });
            AlterColumn("dbo.Carico", "yearId", c => c.Guid());
            CreateIndex("dbo.Carico", "yearId");
            AddForeignKey("dbo.Carico", "yearId", "dbo.Year", "yearId");
        }
    }
}
