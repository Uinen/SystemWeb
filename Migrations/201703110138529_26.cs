namespace SystemWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _26 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Carico",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        pvID = c.Guid(nullable: false),
                        yearId = c.Guid(),
                        Ordine = c.Int(nullable: false),
                        cData = c.DateTime(nullable: false),
                        Documento = c.String(maxLength: 4),
                        Numero = c.String(maxLength: 18),
                        rData = c.DateTime(nullable: false),
                        Emittente = c.String(maxLength: 32),
                        Benzina = c.Int(nullable: false),
                        Gasolio = c.Int(nullable: false),
                        Note = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pv", t => t.pvID, cascadeDelete: true)
                .ForeignKey("dbo.Year", t => t.yearId)
                .Index(t => t.pvID)
                .Index(t => t.yearId);
            
            CreateTable(
                "dbo.Pv",
                c => new
                    {
                        pvID = c.Guid(nullable: false),
                        pvName = c.String(maxLength: 8),
                        pvFlagId = c.Guid(),
                    })
                .PrimaryKey(t => t.pvID)
                .ForeignKey("dbo.Flag", t => t.pvFlagId)
                .Index(t => t.pvFlagId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ProfileId = c.Guid(nullable: false),
                        pvID = c.Guid(nullable: false),
                        CompanyId = c.Guid(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.Pv", t => t.pvID, cascadeDelete: true)
                .ForeignKey("dbo.UserProfiles", t => t.ProfileId, cascadeDelete: true)
                .Index(t => t.ProfileId)
                .Index(t => t.pvID)
                .Index(t => t.CompanyId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.UserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Company",
                c => new
                    {
                        CompanyId = c.Guid(nullable: false),
                        Name = c.String(maxLength: 32),
                        PartitaIva = c.Int(nullable: false),
                        RagioneSocialeId = c.Guid(),
                    })
                .PrimaryKey(t => t.CompanyId)
                .ForeignKey("dbo.RagioneSociale", t => t.RagioneSocialeId)
                .Index(t => t.RagioneSocialeId);
            
            CreateTable(
                "dbo.RagioneSociale",
                c => new
                    {
                        RagioneSocialeId = c.Guid(nullable: false),
                        Nome = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.RagioneSocialeId);
            
            CreateTable(
                "dbo.CompanyTask",
                c => new
                    {
                        CompanyTaskId = c.Guid(nullable: false),
                        UsersId = c.String(nullable: false, maxLength: 128),
                        FieldChiusura = c.String(),
                        FieldDate = c.DateTime(),
                        FieldResult = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyTaskId)
                .ForeignKey("dbo.Users", t => t.UsersId, cascadeDelete: true)
                .Index(t => t.UsersId);
            
            CreateTable(
                "dbo.UserLogin",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Notice",
                c => new
                    {
                        NoticeId = c.Guid(nullable: false),
                        NoticeName = c.String(maxLength: 18),
                        CreateDate = c.DateTime(nullable: false),
                        TextBox = c.String(),
                        UsersId = c.String(nullable: false, maxLength: 128),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.NoticeId)
                .ForeignKey("dbo.Users", t => t.UsersId, cascadeDelete: true)
                .Index(t => t.UsersId);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserArea",
                c => new
                    {
                        UserAreaId = c.Guid(nullable: false),
                        UsersId = c.String(nullable: false, maxLength: 128),
                        UserFieldAccount = c.String(maxLength: 48),
                        UserFieldUsername = c.String(maxLength: 28),
                        UserFieldPassword = c.String(maxLength: 48),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserAreaId)
                .ForeignKey("dbo.Users", t => t.UsersId, cascadeDelete: true)
                .Index(t => t.UsersId);
            
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        ProfileId = c.Guid(nullable: false),
                        ProfileName = c.String(maxLength: 14),
                        ProfileSurname = c.String(maxLength: 14),
                        ProfileAdress = c.String(maxLength: 32),
                        ProfileCity = c.String(maxLength: 14),
                        ProfileZipCode = c.Int(nullable: false),
                        ProfileNation = c.String(maxLength: 14),
                        ProfileInfo = c.String(),
                    })
                .PrimaryKey(t => t.ProfileId);
            
            CreateTable(
                "dbo.Flag",
                c => new
                    {
                        pvFlagId = c.Guid(nullable: false),
                        Nome = c.String(maxLength: 10),
                        Descrizione = c.String(maxLength: 64),
                    })
                .PrimaryKey(t => t.pvFlagId);
            
            CreateTable(
                "dbo.PvProfile",
                c => new
                    {
                        PvProfileId = c.Guid(nullable: false),
                        pvID = c.Guid(nullable: false),
                        Indirizzo = c.String(maxLength: 32),
                        Città = c.String(maxLength: 24),
                        Nazione = c.String(maxLength: 14),
                        Cap = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PvProfileId)
                .ForeignKey("dbo.Pv", t => t.pvID, cascadeDelete: true)
                .Index(t => t.pvID);
            
            CreateTable(
                "dbo.PvTank",
                c => new
                    {
                        PvTankId = c.Guid(nullable: false),
                        pvID = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                        Modello = c.String(maxLength: 14),
                        LastDate = c.DateTime(nullable: false),
                        Capienza = c.Int(nullable: false),
                        Giacenza = c.Int(nullable: false),
                        Descrizione = c.String(),
                    })
                .PrimaryKey(t => t.PvTankId)
                .ForeignKey("dbo.Product", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Pv", t => t.pvID, cascadeDelete: true)
                .Index(t => t.pvID)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        ProductId = c.Guid(nullable: false),
                        Nome = c.String(maxLength: 14),
                        Peso = c.Single(nullable: false),
                        Prezzo = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.ProductId);
            
            CreateTable(
                "dbo.PvCali",
                c => new
                    {
                        PvCaliId = c.Guid(nullable: false),
                        PvTankId = c.Guid(nullable: false),
                        Value = c.Int(nullable: false),
                        FieldDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PvCaliId)
                .ForeignKey("dbo.PvTank", t => t.PvTankId, cascadeDelete: true)
                .Index(t => t.PvTankId);
            
            CreateTable(
                "dbo.PvDeficienze",
                c => new
                    {
                        PvDefId = c.Guid(nullable: false),
                        PvTankId = c.Guid(nullable: false),
                        Value = c.Int(nullable: false),
                        FieldDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PvDefId)
                .ForeignKey("dbo.PvTank", t => t.PvTankId, cascadeDelete: true)
                .Index(t => t.PvTankId);
            
            CreateTable(
                "dbo.PvTankDesc",
                c => new
                    {
                        PvTankDescId = c.Guid(nullable: false),
                        PvTankId = c.Guid(nullable: false),
                        PvTankCM = c.Single(nullable: false),
                        PvTankLT = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.PvTankDescId)
                .ForeignKey("dbo.PvTank", t => t.PvTankId, cascadeDelete: true)
                .Index(t => t.PvTankId);
            
            CreateTable(
                "dbo.Year",
                c => new
                    {
                        yearId = c.Guid(nullable: false),
                        Anno = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.yearId);
            
            CreateTable(
                "dbo.Dispenser",
                c => new
                    {
                        DispenserId = c.Guid(nullable: false),
                        Modello = c.String(maxLength: 32),
                        PvTankId = c.Guid(),
                        isActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.DispenserId)
                .ForeignKey("dbo.PvTank", t => t.PvTankId)
                .Index(t => t.PvTankId);
            
            CreateTable(
                "dbo.PvErogatori",
                c => new
                    {
                        PvErogatoriId = c.Guid(nullable: false),
                        pvID = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                        DispenserId = c.Guid(nullable: false),
                        FieldDate = c.DateTime(nullable: false),
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PvErogatoriId)
                .ForeignKey("dbo.Dispenser", t => t.DispenserId, cascadeDelete: true)
                .ForeignKey("dbo.Product", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Pv", t => t.pvID, cascadeDelete: true)
                .Index(t => t.pvID)
                .Index(t => t.ProductId)
                .Index(t => t.DispenserId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.UsersImage",
                c => new
                    {
                        UsersImageId = c.Guid(nullable: false),
                        ImagePath = c.String(),
                        UploadDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UsersImageId);
            
            CreateTable(
                "dbo.IdentityUserLogin",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(),
                        ProviderKey = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.IdentityRole",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IdentityUserRole",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IdentityRole_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.RoleId, t.UserId })
                .ForeignKey("dbo.IdentityRole", t => t.IdentityRole_Id)
                .Index(t => t.IdentityRole_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IdentityUserRole", "IdentityRole_Id", "dbo.IdentityRole");
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Dispenser", "PvTankId", "dbo.PvTank");
            DropForeignKey("dbo.PvErogatori", "pvID", "dbo.Pv");
            DropForeignKey("dbo.PvErogatori", "ProductId", "dbo.Product");
            DropForeignKey("dbo.PvErogatori", "DispenserId", "dbo.Dispenser");
            DropForeignKey("dbo.Carico", "yearId", "dbo.Year");
            DropForeignKey("dbo.PvTankDesc", "PvTankId", "dbo.PvTank");
            DropForeignKey("dbo.PvDeficienze", "PvTankId", "dbo.PvTank");
            DropForeignKey("dbo.PvCali", "PvTankId", "dbo.PvTank");
            DropForeignKey("dbo.PvTank", "pvID", "dbo.Pv");
            DropForeignKey("dbo.PvTank", "ProductId", "dbo.Product");
            DropForeignKey("dbo.PvProfile", "pvID", "dbo.Pv");
            DropForeignKey("dbo.Pv", "pvFlagId", "dbo.Flag");
            DropForeignKey("dbo.Carico", "pvID", "dbo.Pv");
            DropForeignKey("dbo.Users", "ProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.UserArea", "UsersId", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "pvID", "dbo.Pv");
            DropForeignKey("dbo.Notice", "UsersId", "dbo.Users");
            DropForeignKey("dbo.UserLogin", "UserId", "dbo.Users");
            DropForeignKey("dbo.CompanyTask", "UsersId", "dbo.Users");
            DropForeignKey("dbo.Users", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Company", "RagioneSocialeId", "dbo.RagioneSociale");
            DropForeignKey("dbo.UserClaims", "UserId", "dbo.Users");
            DropIndex("dbo.IdentityUserRole", new[] { "IdentityRole_Id" });
            DropIndex("dbo.Roles", "RoleNameIndex");
            DropIndex("dbo.PvErogatori", new[] { "DispenserId" });
            DropIndex("dbo.PvErogatori", new[] { "ProductId" });
            DropIndex("dbo.PvErogatori", new[] { "pvID" });
            DropIndex("dbo.Dispenser", new[] { "PvTankId" });
            DropIndex("dbo.PvTankDesc", new[] { "PvTankId" });
            DropIndex("dbo.PvDeficienze", new[] { "PvTankId" });
            DropIndex("dbo.PvCali", new[] { "PvTankId" });
            DropIndex("dbo.PvTank", new[] { "ProductId" });
            DropIndex("dbo.PvTank", new[] { "pvID" });
            DropIndex("dbo.PvProfile", new[] { "pvID" });
            DropIndex("dbo.UserArea", new[] { "UsersId" });
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.Notice", new[] { "UsersId" });
            DropIndex("dbo.UserLogin", new[] { "UserId" });
            DropIndex("dbo.CompanyTask", new[] { "UsersId" });
            DropIndex("dbo.Company", new[] { "RagioneSocialeId" });
            DropIndex("dbo.UserClaims", new[] { "UserId" });
            DropIndex("dbo.Users", "UserNameIndex");
            DropIndex("dbo.Users", new[] { "CompanyId" });
            DropIndex("dbo.Users", new[] { "pvID" });
            DropIndex("dbo.Users", new[] { "ProfileId" });
            DropIndex("dbo.Pv", new[] { "pvFlagId" });
            DropIndex("dbo.Carico", new[] { "yearId" });
            DropIndex("dbo.Carico", new[] { "pvID" });
            DropTable("dbo.IdentityUserRole");
            DropTable("dbo.IdentityRole");
            DropTable("dbo.IdentityUserLogin");
            DropTable("dbo.UsersImage");
            DropTable("dbo.Roles");
            DropTable("dbo.PvErogatori");
            DropTable("dbo.Dispenser");
            DropTable("dbo.Year");
            DropTable("dbo.PvTankDesc");
            DropTable("dbo.PvDeficienze");
            DropTable("dbo.PvCali");
            DropTable("dbo.Product");
            DropTable("dbo.PvTank");
            DropTable("dbo.PvProfile");
            DropTable("dbo.Flag");
            DropTable("dbo.UserProfiles");
            DropTable("dbo.UserArea");
            DropTable("dbo.UserRoles");
            DropTable("dbo.Notice");
            DropTable("dbo.UserLogin");
            DropTable("dbo.CompanyTask");
            DropTable("dbo.RagioneSociale");
            DropTable("dbo.Company");
            DropTable("dbo.UserClaims");
            DropTable("dbo.Users");
            DropTable("dbo.Pv");
            DropTable("dbo.Carico");
        }
    }
}
