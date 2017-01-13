namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _18 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Year", new[] { "yearNumber" });
            DropIndex("dbo.Day", new[] { "nDay" });
            DropIndex("dbo.Month", new[] { "saltMonth" });
            DropIndex("dbo.Month", new[] { "MonthName" });
            AddColumn("dbo.Year", "Anno", c => c.DateTime(nullable: false));
            DropColumn("dbo.Year", "yearNumber");
            DropTable("dbo.Day");
            DropTable("dbo.Month");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Month",
                c => new
                    {
                        monthId = c.Guid(nullable: false),
                        saltMonth = c.String(maxLength: 4),
                        MonthName = c.String(maxLength: 14),
                    })
                .PrimaryKey(t => t.monthId);
            
            CreateTable(
                "dbo.Day",
                c => new
                    {
                        dayId = c.Guid(nullable: false),
                        nDay = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.dayId);
            
            AddColumn("dbo.Year", "yearNumber", c => c.Int(nullable: false));
            DropColumn("dbo.Year", "Anno");
            CreateIndex("dbo.Month", "MonthName", unique: true);
            CreateIndex("dbo.Month", "saltMonth", unique: true);
            CreateIndex("dbo.Day", "nDay", unique: true);
            CreateIndex("dbo.Year", "yearNumber");
        }
    }
}
