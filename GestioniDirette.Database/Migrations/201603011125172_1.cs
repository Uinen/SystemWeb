namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompanyTask",
                c => new
                    {
                        CompanyTaskId = c.Guid(nullable: false),
                        UsersId = c.String(nullable: false, maxLength: 128),
                        FieldChiusura = c.String(),
                        FieldDate = c.DateTime(nullable: false, storeType: "date"),
                        FieldResult = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyTaskId)
                .ForeignKey("dbo.Users", t => t.UsersId, cascadeDelete: true)
                .Index(t => t.UsersId);

            CreateTable(
                "dbo.UserArea",
                c => new
                    {
                        UserAreaId = c.Guid(nullable: false),
                        UsersId = c.String(nullable: false, maxLength: 128),
                        UserFieldAccount = c.String(maxLength: 48),
                        UserFieldUsername = c.String(maxLength: 28),
                        UserFieldPassword = c.String(maxLength: 48),
                        CreateDate = c.DateTime(nullable: false, storeType: "date"),
                    })
                .PrimaryKey(t => t.UserAreaId)
                .ForeignKey("dbo.Users", t => t.UsersId, cascadeDelete: true)
                .Index(t => t.UsersId);
        }
        public override void Down()
        {
            DropForeignKey("dbo.UserArea", "UsersId", "dbo.Users");
            DropForeignKey("dbo.CompanyTask", "UsersId", "dbo.Users");
            DropIndex("dbo.UserArea", new[] { "UsersId" });
            DropIndex("dbo.CompanyTask", new[] { "UsersId" });
            DropTable("dbo.UserArea");
            DropTable("dbo.CompanyTask");
        }
    }
}
