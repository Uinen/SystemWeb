namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _4 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Day",
                c => new
                    {
                        dayId = c.Guid(nullable: false),
                        nDay = c.Int(nullable: true),
                    })
                .PrimaryKey(t => t.dayId)
                .Index(t => t.nDay, unique: true);
            
            CreateTable(
                "dbo.Month",
                c => new
                    {
                        monthId = c.Guid(nullable: false),
                        OneLetterMonth = c.String(maxLength: 1, nullable: true),
                        saltMonth = c.String(maxLength: 4, nullable: true),
                        MonthName = c.String(maxLength: 14, nullable: true),
                    })
                .PrimaryKey(t => t.monthId)
                .Index(t => t.OneLetterMonth, unique: true)
                .Index(t => t.saltMonth, unique: true)
                .Index(t => t.MonthName, unique: true);
            
            CreateTable(
                "dbo.Year",
                c => new
                    {
                        yearId = c.Guid(nullable: false),
                        year = c.Int(nullable: true),
                    })
                .PrimaryKey(t => t.yearId)
                .Index(t => t.year, unique: true);
            
            AddColumn("dbo.CompanyTask", "dayId", c => c.Guid(nullable: true));
            AddColumn("dbo.CompanyTask", "monthId", c => c.Guid(nullable: true));
            AddColumn("dbo.CompanyTask", "yearId", c => c.Guid(nullable: true));
            AlterColumn("dbo.CompanyTask", "FieldDate", c => c.DateTime(storeType: "date"));
            CreateIndex("dbo.CompanyTask", "dayId");
            CreateIndex("dbo.CompanyTask", "monthId");
            CreateIndex("dbo.CompanyTask", "yearId");
            AddForeignKey("dbo.CompanyTask", "dayId", "dbo.Day", "dayId", cascadeDelete: true);
            AddForeignKey("dbo.CompanyTask", "monthId", "dbo.Month", "monthId", cascadeDelete: true);
            AddForeignKey("dbo.CompanyTask", "yearId", "dbo.Year", "yearId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompanyTask", "yearId", "dbo.Year");
            DropForeignKey("dbo.CompanyTask", "monthId", "dbo.Month");
            DropForeignKey("dbo.CompanyTask", "dayId", "dbo.Day");
            DropIndex("dbo.Year", new[] { "year" });
            DropIndex("dbo.Month", new[] { "MonthName" });
            DropIndex("dbo.Month", new[] { "saltMonth" });
            DropIndex("dbo.Month", new[] { "OneLetterMonth" });
            DropIndex("dbo.Day", new[] { "nDay" });
            DropIndex("dbo.CompanyTask", new[] { "yearId" });
            DropIndex("dbo.CompanyTask", new[] { "monthId" });
            DropIndex("dbo.CompanyTask", new[] { "dayId" });
            AlterColumn("dbo.CompanyTask", "FieldDate", c => c.DateTime(nullable: true, storeType: "date"));
            DropColumn("dbo.CompanyTask", "yearId");
            DropColumn("dbo.CompanyTask", "monthId");
            DropColumn("dbo.CompanyTask", "dayId");
            DropTable("dbo.Year");
            DropTable("dbo.Month");
            DropTable("dbo.Day");
        }
    }
}
