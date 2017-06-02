namespace GestioniDirette.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carico", "HiQb", c => c.Int());
            AddColumn("dbo.Carico", "HiQd", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Carico", "HiQd");
            DropColumn("dbo.Carico", "HiQb");
        }
    }
}
