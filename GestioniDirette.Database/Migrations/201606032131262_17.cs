namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _17 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PvErogatori", "FieldDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PvErogatori", "FieldDate", c => c.DateTime());
        }
    }
}
