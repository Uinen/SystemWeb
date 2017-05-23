namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carico", "yearId", c => c.Guid(nullable: true));
            CreateIndex("dbo.Carico", "yearId");
            AddForeignKey("dbo.Carico", "yearId", "dbo.Year", "yearId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Carico", "yearId", "dbo.Year");
            DropIndex("dbo.Carico", new[] { "yearId" });
            DropColumn("dbo.Carico", "yearId");
        }
    }
}
