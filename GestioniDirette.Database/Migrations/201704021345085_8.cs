namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _8 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserProfiles", "ProfileCity", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserProfiles", "ProfileCity", c => c.String(maxLength: 14));
        }
    }
}
