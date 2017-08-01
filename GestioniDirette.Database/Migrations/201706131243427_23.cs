namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _23 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PayPal",
                c => new
                    {
                        PayPalID = c.Guid(nullable: false),
                        UserID = c.String(nullable: false, maxLength: 128),
                        PlanID = c.String(),
                        Nome = c.String(),
                        Stato = c.String(),
                        Tipo = c.String(),
                        CreatedDate = c.String(),
                        Update = c.String(),
                    })
                .PrimaryKey(t => t.PayPalID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            AddColumn("dbo.Users", "isPremium", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PayPal", "UserID", "dbo.Users");
            DropIndex("dbo.PayPal", new[] { "UserID" });
            DropColumn("dbo.Users", "isPremium");
            DropTable("dbo.PayPal");
        }
    }
}
