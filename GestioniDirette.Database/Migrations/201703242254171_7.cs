namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _7 : DbMigration
    {
        public override void Up()
        {
            /*DropForeignKey("dbo.Users", "UsersImageId", "dbo.UsersImage");
            DropIndex("dbo.Users", new[] { "UsersImageId" });
            CreateTable(
                "dbo.FilePath",
                c => new
                    {
                        FilePathID = c.Guid(nullable: false),
                        FileName = c.String(maxLength: 255),
                        FileType = c.Int(nullable: false),
                        UploadDate = c.DateTime(nullable: false),
                        UserID = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.FilePathID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            AddColumn("dbo.UsersImage", "UsersImageName", c => c.String(maxLength: 255));
            AddColumn("dbo.UsersImage", "ContentType", c => c.String(maxLength: 100));
            AddColumn("dbo.UsersImage", "Content", c => c.Binary());
            AddColumn("dbo.UsersImage", "FileType", c => c.Int(nullable: false));
            AddColumn("dbo.UsersImage", "ProfileID", c => c.Guid(nullable: false));
            AlterColumn("dbo.UsersImage", "UploadDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.UsersImage", "ProfileID");
            AddForeignKey("dbo.UsersImage", "ProfileID", "dbo.UserProfiles", "ProfileId", cascadeDelete: true);
            DropColumn("dbo.Users", "UsersImageId");
            DropColumn("dbo.UsersImage", "ImagePath");*/
        }
        
        public override void Down()
        {
            /*AddColumn("dbo.UsersImage", "ImagePath", c => c.String());
            AddColumn("dbo.Users", "UsersImageId", c => c.Guid());
            DropForeignKey("dbo.UsersImage", "ProfileID", "dbo.UserProfiles");
            DropForeignKey("dbo.FilePath", "UserID", "dbo.Users");
            DropIndex("dbo.UsersImage", new[] { "ProfileID" });
            DropIndex("dbo.FilePath", new[] { "UserID" });
            AlterColumn("dbo.UsersImage", "UploadDate", c => c.DateTime());
            DropColumn("dbo.UsersImage", "ProfileID");
            DropColumn("dbo.UsersImage", "FileType");
            DropColumn("dbo.UsersImage", "Content");
            DropColumn("dbo.UsersImage", "ContentType");
            DropColumn("dbo.UsersImage", "UsersImageName");
            DropTable("dbo.FilePath");
            CreateIndex("dbo.Users", "UsersImageId");
            AddForeignKey("dbo.Users", "UsersImageId", "dbo.UsersImage", "UsersImageId");*/
        }
    }
}
