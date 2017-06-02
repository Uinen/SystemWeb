namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PvErogatori", "dayId", c => c.Guid(nullable: false));
            AddColumn("dbo.PvErogatori", "monthId", c => c.Guid(nullable: false));
            AddColumn("dbo.PvErogatori", "yearId", c => c.Guid(nullable: false));
            CreateIndex("dbo.PvErogatori", "dayId");
            CreateIndex("dbo.PvErogatori", "monthId");
            CreateIndex("dbo.PvErogatori", "yearId");
            AddForeignKey("dbo.PvErogatori", "dayId", "dbo.Day", "dayId", cascadeDelete: true);
            AddForeignKey("dbo.PvErogatori", "monthId", "dbo.Month", "monthId", cascadeDelete: true);
            AddForeignKey("dbo.PvErogatori", "yearId", "dbo.Year", "yearId", cascadeDelete: true);
            DropColumn("dbo.PvErogatori", "Data");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PvErogatori", "Data", c => c.DateTime(nullable: false, storeType: "date"));
            DropForeignKey("dbo.PvErogatori", "yearId", "dbo.Year");
            DropForeignKey("dbo.PvErogatori", "monthId", "dbo.Month");
            DropForeignKey("dbo.PvErogatori", "dayId", "dbo.Day");
            DropIndex("dbo.PvErogatori", new[] { "yearId" });
            DropIndex("dbo.PvErogatori", new[] { "monthId" });
            DropIndex("dbo.PvErogatori", new[] { "dayId" });
            DropColumn("dbo.PvErogatori", "yearId");
            DropColumn("dbo.PvErogatori", "monthId");
            DropColumn("dbo.PvErogatori", "dayId");
        }
    }
}
