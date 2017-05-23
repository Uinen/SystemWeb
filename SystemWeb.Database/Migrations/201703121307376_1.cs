namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            /*
            CreateTable(
                "dbo.UsersImage",
                c => new
                    {
                        UsersImageId = c.Guid(nullable: false),
                        ImagePath = c.String(),
                        UploadDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UsersImageId);*/
        }
        
        public override void Down()
        {
            //DropTable("dbo.UsersImage");
        }
    }
}
