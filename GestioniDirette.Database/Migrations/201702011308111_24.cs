namespace SystemWeb.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class _24 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dispenser", "isActive", c => c.Boolean(nullable: true));
        }
        
        public override void Down()
        {
        }
    }
}
