namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _12 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PvErogatori", "dayId", "dbo.Day");
            DropForeignKey("dbo.PvErogatori", "monthId", "dbo.Month");
            DropForeignKey("dbo.PvErogatori", "yearId", "dbo.Year");
            DropForeignKey("dbo.CompanyTask", "dayId", "dbo.Day");
            DropForeignKey("dbo.CompanyTask", "monthId", "dbo.Month");
            DropForeignKey("dbo.CompanyTask", "yearId", "dbo.Year");
            DropIndex("dbo.CompanyTask", new[] { "dayId" });
            DropIndex("dbo.CompanyTask", new[] { "monthId" });
            DropIndex("dbo.CompanyTask", new[] { "yearId" });
            DropIndex("dbo.PvErogatori", new[] { "dayId" });
            DropIndex("dbo.PvErogatori", new[] { "monthId" });
            DropIndex("dbo.PvErogatori", new[] { "yearId" });
            AddColumn("dbo.PvErogatori", "FieldDate", c => c.DateTime());
            AlterColumn("dbo.Carico", "cData", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Carico", "rData", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CompanyTask", "FieldDate", c => c.DateTime());
            AlterColumn("dbo.Notice", "CreateDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.UserArea", "CreateDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.CompanyTask", "dayId");
            DropColumn("dbo.CompanyTask", "monthId");
            DropColumn("dbo.CompanyTask", "yearId");
            DropColumn("dbo.PvErogatori", "dayId");
            DropColumn("dbo.PvErogatori", "monthId");
            DropColumn("dbo.PvErogatori", "yearId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PvErogatori", "yearId", c => c.Guid(nullable: false));
            AddColumn("dbo.PvErogatori", "monthId", c => c.Guid(nullable: false));
            AddColumn("dbo.PvErogatori", "dayId", c => c.Guid(nullable: false));
            AddColumn("dbo.CompanyTask", "yearId", c => c.Guid(nullable: false));
            AddColumn("dbo.CompanyTask", "monthId", c => c.Guid(nullable: false));
            AddColumn("dbo.CompanyTask", "dayId", c => c.Guid(nullable: false));
            AlterColumn("dbo.UserArea", "CreateDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.Notice", "CreateDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.CompanyTask", "FieldDate", c => c.DateTime(storeType: "date"));
            AlterColumn("dbo.Carico", "rData", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.Carico", "cData", c => c.DateTime(nullable: false, storeType: "date"));
            DropColumn("dbo.PvErogatori", "FieldDate");
            CreateIndex("dbo.PvErogatori", "yearId");
            CreateIndex("dbo.PvErogatori", "monthId");
            CreateIndex("dbo.PvErogatori", "dayId");
            CreateIndex("dbo.CompanyTask", "yearId");
            CreateIndex("dbo.CompanyTask", "monthId");
            CreateIndex("dbo.CompanyTask", "dayId");
            AddForeignKey("dbo.CompanyTask", "yearId", "dbo.Year", "yearId", cascadeDelete: true);
            AddForeignKey("dbo.CompanyTask", "monthId", "dbo.Month", "monthId", cascadeDelete: true);
            AddForeignKey("dbo.CompanyTask", "dayId", "dbo.Day", "dayId", cascadeDelete: true);
            AddForeignKey("dbo.PvErogatori", "yearId", "dbo.Year", "yearId", cascadeDelete: true);
            AddForeignKey("dbo.PvErogatori", "monthId", "dbo.Month", "monthId", cascadeDelete: true);
            AddForeignKey("dbo.PvErogatori", "dayId", "dbo.Day", "dayId", cascadeDelete: true);
        }
    }
}
