namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _14 : DbMigration
    {
        public override void Up()
        {

        }
        
        public override void Down()
        {
            DropColumn("dbo.Carico", "LoadViewModels_tData");
            DropColumn("dbo.Carico", "LoadViewModels_fData");
            DropColumn("dbo.Carico", "LoadViewModels_DieselTotalAmount");
            DropColumn("dbo.Carico", "LoadViewModels_SSPBTotalAmount");
            DropColumn("dbo.Carico", "LoadViewModels_vNote");
            DropColumn("dbo.Carico", "LoadViewModels_vGasolio");
            DropColumn("dbo.Carico", "LoadViewModels_vBenzina");
            DropColumn("dbo.Carico", "LoadViewModels_vEmittente");
            DropColumn("dbo.Carico", "LoadViewModels_vRData");
            DropColumn("dbo.Carico", "LoadViewModels_vNumero");
            DropColumn("dbo.Carico", "LoadViewModels_vCData");
            DropColumn("dbo.Carico", "LoadViewModels_vOrdine");
        }
    }
}
