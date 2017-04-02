namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _6 : DbMigration
    {
        public override void Up()
        {
            /*DropForeignKey("dbo.Users", "UsersImageId", "dbo.UsersImage");
            DropIndex("dbo.Users", new[] { "UsersImageId" });
            AlterColumn("dbo.Users", "UsersImageId", c => c.Guid());
            CreateIndex("dbo.Users", "UsersImageId");
            AddForeignKey("dbo.Users", "UsersImageId", "dbo.UsersImage", "UsersImageId");*/
        }
        
        public override void Down()
        {
            /*DropForeignKey("dbo.Users", "UsersImageId", "dbo.UsersImage");
            DropIndex("dbo.Users", new[] { "UsersImageId" });
            AlterColumn("dbo.Users", "UsersImageId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Users", "UsersImageId");
            AddForeignKey("dbo.Users", "UsersImageId", "dbo.UsersImage", "UsersImageId", cascadeDelete: true);*/
        }
    }
}
