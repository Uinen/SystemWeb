namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _4 : DbMigration
    {
        public override void Up()
        {
            //CreateIndex("dbo.Users", "UsersImageId");
            //AddForeignKey("dbo.Users", "UsersImageId", "dbo.UsersImage", "UsersImageId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.Users", "UsersImageId", "dbo.UsersImage");
            //DropIndex("dbo.Users", new[] { "UsersImageId" });
        }
    }
}
