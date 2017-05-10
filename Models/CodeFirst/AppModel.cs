
#region Direttive 
using System;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web;
#endregion

namespace SystemWeb.Models
{
    #region Interfacce
    public class ApplicationUserLogin : IdentityUserLogin<string> { }
    public class ApplicationUserClaim : IdentityUserClaim<string> { }
    public class ApplicationUserRole : IdentityUserRole<string>
    {

    }
    #endregion

    #region User
    public sealed class ApplicationUser : IdentityUser<string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public Guid? ProfileId { get; set; }
        public UserProfiles UserProfiles { get; set; }
        public Guid? pvID { get; set; }
        public Pv Pv { get; set; }
        public Guid? CompanyId { get; set; }
        public DateTime CreateDate { get; set; }
        public Company Company { get; set; }
        
        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();
            Notice = new HashSet<Notice>();
            UserArea = new HashSet<UserArea>();
            CompanyTask = new HashSet<CompanyTask>();
            FilePaths = new HashSet<FilePath>();
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            var userIdentity = await manager
                .CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
        public ICollection<Notice> Notice { get; set; }
        public ICollection<UserArea> UserArea { get; set; }
        public ICollection<CompanyTask> CompanyTask { get; set; }
        public ICollection<FilePath> FilePaths { get; set; }
    }
    #endregion

    #region Role
    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    {
        public ApplicationRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        public ApplicationRole(string name)
            : this()
        {
            Name = name;
        }
    }
    #endregion

    #region User Image
    public class UsersImage
    {
        public UsersImage()
        {
            UsersImageID = Guid.NewGuid();
        }
        [Key]
        public Guid UsersImageID { get; set; }
        [StringLength(255)]
        public string UsersImageName { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public FileType FileType { get; set; }
        public DateTime UploadDate { get; set; }
        public Guid ProfileID { get; set; }
        public UserProfiles UserProfiles { get; set; }
    }
    #endregion

    #region Carico
    public class Carico
    {
        public Carico()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        public Guid pvID { get; set; }
        public Guid? yearId { get; set; }
        public int Ordine { get; set; }
        public DateTime cData { get; set; }
        [MaxLength(4)]
        public string Documento { get; set; }
        [MaxLength(18)]
        public string Numero { get; set; }
        public DateTime rData { get; set; }
        [MaxLength(32)]
        public string Emittente { get; set; }
        public int Benzina { get; set; }
        public int Gasolio { get; set; }
        public int? HiQb { get; set; }
        public int? HiQd { get; set; }
        public string Note { get; set; }
        public Pv Pv { get; set; }
        public Year Year { get; set; }

        [NotMapped]
        public string ord { get; set; }
        [NotMapped]
        public string sspb { get; set; }
        [NotMapped]
        public string dsl { get; set; }
        [NotMapped]
        public string date { get; set; }
    }
    #endregion

    #region Company
    public class Company
    {
        public Company()
        {
            CompanyId = Guid.NewGuid();
            ApplicationUser = new HashSet<ApplicationUser>();
        }
        [Key]
        public Guid CompanyId { get; set; }
        [MaxLength(32)]
        public string Name { get; set; }
        public int PartitaIva { get; set; }
        public Guid? RagioneSocialeId { get; set; }
        public RagioneSociale RagioneSociale { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<ApplicationUser> ApplicationUser { get; set; }
    }
    #endregion

    #region Company Task
    public class CompanyTask
    {
        public CompanyTask()
        {
            CompanyTaskId = Guid.NewGuid();
        }
        [Key]
        public Guid CompanyTaskId { get; set; }
        [MaxLength(128)]
        public string UsersId { get; set; }
        public string FieldChiusura { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yy}", ApplyFormatInEditMode = true)]
        public DateTime? FieldDate { get; set; }
        public float FieldResult { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
    #endregion

    #region Ragione Sociale
    public class RagioneSociale
    {
        public RagioneSociale()
        {
            RagioneSocialeId = Guid.NewGuid();
        }
        [Key]
        public Guid RagioneSocialeId { get; set; }
        [MaxLength(10)]
        public string Nome { get; set; }
    }
    #endregion

    #region Pv Erogatori
    public class PvErogatori
    {
        public PvErogatori()
        {
            PvErogatoriId = Guid.NewGuid();
        }
        [Key]
        public Guid PvErogatoriId { get; set; }
        public Guid pvID { get; set; }
        public Guid ProductId { get; set; }
        public Guid DispenserId { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yy}", ApplyFormatInEditMode = true)]
        public DateTime FieldDate { get; set; }
        public int Value { get; set; }
        public Product Product { get; set; }
        public Dispenser Dispenser { get; set; }
        public Pv Pv { get; set; }
        [NotMapped]
        public string sspb { get; set; }
        [NotMapped]
        public string dsl { get; set; }
    }
    #endregion

    #region Flag
    public class Flag
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Flag()
        {
            Pv = new HashSet<Pv>();
            pvFlagId = Guid.NewGuid();
        }
        [Key]
        public Guid pvFlagId { get; set; }
        [MaxLength(10)]
        public string Nome { get; set; }
        [MaxLength(64)]
        public string Descrizione { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Pv> Pv { get; set; }
    }
    #endregion

    #region Product
    public class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            PvTank = new HashSet<PvTank>();
            ProductId = Guid.NewGuid();
        }
        [Key]
        public Guid ProductId { get; set; }
        [MaxLength(14)]
        public string Nome { get; set; }
        public float Peso { get; set; }
        public float Prezzo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<PvTank> PvTank { get; set; }
    }
    #endregion

    #region Pv
    public class Pv
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pv()
        {
            PvTank = new HashSet<PvTank>();
            Carico = new HashSet<Carico>();
            PvProfile = new HashSet<PvProfile>();
            ApplicationUser = new HashSet<ApplicationUser>();
            Cartissima = new HashSet<Cartissima>();
            pvID = Guid.NewGuid();
        }

        [Key]
        public Guid pvID { get; set; }
        [MaxLength(8)]
        public string pvName { get; set; }
        public Guid? pvFlagId { get; set; }
        public Flag Flag { get; set; }
        public ICollection<PvTank> PvTank { get; set; }
        public ICollection<Carico> Carico { get; set; }
        public ICollection<PvProfile> PvProfile { get; set; }
        public ICollection<ApplicationUser> ApplicationUser { get; set; }
        public ICollection<Cartissima> Cartissima { get; set; }
    }
    #endregion

    #region Pv Profile
    public class PvProfile
    {
        public PvProfile()
        {
            PvProfileId = Guid.NewGuid();
        }
        [Key]
        public Guid PvProfileId { get; set; }
        public Guid pvID { get; set; }
        [MaxLength(32)]
        public string Indirizzo { get; set; }
        [MaxLength(24)]
        public string Città { get; set; }
        [MaxLength(14)]
        public string Nazione { get; set; }
        public int Cap { get; set; }
        public Pv Pv { get; set; }
    }
    #endregion

    #region Pv Tank
    public class PvTank
    {
        public PvTank()
        {
            PvTankId = Guid.NewGuid();
            PvTankDesc = new HashSet<PvTankDesc>();
            PvDeficienze = new HashSet<PvDeficienze>();
            PvCali = new HashSet<PvCali>();
        }
        [Key]
        public Guid PvTankId { get; set; }
        public Guid pvID { get; set; }
        public Guid ProductId { get; set; }
        [MaxLength(14)]
        public string Modello { get; set; }
        public DateTime LastDate { get; set; }
        public int Capienza { get; set; }
        public int Giacenza { get; set; }
        public string Descrizione { get; set; }
        public Product Product { get; set; }
        public Pv Pv { get; set; }
        public ICollection<PvTankDesc> PvTankDesc { get; set; }
        public ICollection<PvDeficienze> PvDeficienze { get; set; }
        public ICollection<PvCali> PvCali { get; set; }
    }
    #endregion

    #region Pv Deficienze
    public class PvDeficienze
    {
        public PvDeficienze()
        {
            PvDefId = Guid.NewGuid();
        }

        [Key]
        public Guid PvDefId { get; set; }
        public Guid PvTankId { get; set; }
        public PvTank PvTank { get; set; }
        public int Value { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yy}", ApplyFormatInEditMode = true)]
        public DateTime FieldDate { get; set; }
    }
    #endregion

    #region Pv Cali
    public class PvCali
    {
        public PvCali()
        {
            PvCaliId = Guid.NewGuid();
        }

        [Key]
        public Guid PvCaliId { get; set; }
        public Guid PvTankId { get; set; }
        public PvTank PvTank { get; set; }
        public int Value { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yy}", ApplyFormatInEditMode = true)]
        public DateTime FieldDate { get; set; }
    }
    #endregion

    #region Pv Tank Desc
    public class PvTankDesc
    {
        public PvTankDesc()
        {
            PvTankDescId = Guid.NewGuid();
        }

        [Key]
        public Guid PvTankDescId { get; set; }
        public Guid PvTankId { get; set; }
        public float PvTankCM { get; set; }
        public float PvTankLT { get; set; }
        public PvTank PvTank { get; set; }
    }
    #endregion

    #region User Profile
    public class UserProfiles
    {
        public UserProfiles()
        {
            ProfileId = Guid.NewGuid();
            UsersImage = new HashSet<UsersImage>();
        }
        [Key]
        public Guid ProfileId { get; set; }
        [MaxLength(14)]
        public string ProfileName { get; set; }
        [MaxLength(14)]
        public string ProfileSurname { get; set; }
        [MaxLength(32)]
        public string ProfileAdress { get; set; }
        [MaxLength(32)]
        public string ProfileCity { get; set; }
        public int ProfileZipCode { get; set; }
        [MaxLength(14)]
        public string ProfileNation { get; set; }
        public string ProfileInfo { get; set; }
        [Display(Name = "Nome completo")]
        public string FullName
        {
            get
            {
                return ProfileSurname + " " + ProfileName;
            }
        }

        [Display(Name = "Indirizzo completo")]
        public string FullAdress
        {
            get
            {
                return ProfileAdress + ", " + ProfileCity + "- (" + ProfileZipCode + ") -" + ProfileNation;
            }
        }
        public ICollection<ApplicationUser> ApplicationUser { get; set; }
        public ICollection<UsersImage> UsersImage { get; set; }
    }
    #endregion

    #region Dispenser
    public class Dispenser
    {
        public Dispenser()
        {
            DispenserId = Guid.NewGuid();
            PvErogatori = new HashSet<PvErogatori>();
        }
        [Key]
        public Guid DispenserId { get; set; }
        [MaxLength(32)]
        public string Modello { get; set; }
        public Guid? PvTankId { get; set; }
        public PvTank PvTank { get; set; }
        public ICollection<PvErogatori> PvErogatori { get; set; }
        public bool? isActive { get; set; }
    }
    #endregion

    #region Notice
    public class Notice
    {
        public Notice()
        {
            NoticeId = Guid.NewGuid();
        }
        [Key]
        public Guid NoticeId { get; set; }
        [MaxLength(18)]
        public string NoticeName { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yy}", ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; }
        public string TextBox { get; set; }
        [MaxLength(128)]
        public string UsersId { get; set; }
        public string Description { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public override string ToString()
        {
            return string.Format(
                "NoticeName: {0}, CreateDate: {1}, TextBox: {2}, UsersId: {3}, Description: {4}", NoticeName, CreateDate, TextBox, UsersId, Description);
        }

    }
    #endregion

    #region User Area
    public class UserArea
    {
        public UserArea()
        {
            UserAreaId = Guid.NewGuid();
        }
        [Key]
        public Guid UserAreaId { get; set; }
        [MaxLength(128)]
        public string UsersId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [MaxLength(48)]
        public string UserFieldAccount { get; set; }
        [MaxLength(28)]
        public string UserFieldUsername { get; set; }
        [MaxLength(48)]
        public string UserFieldPassword { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yy}", ApplyFormatInEditMode = true)]
        public DateTime CreateDate { get; set; }
    }
    #endregion

    #region Year
    public class Year
    {
        public Year()
        {
            yearId = Guid.NewGuid();
            Carico = new HashSet<Carico>();
        }
        [Key]
        public Guid yearId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Anno { get; set; }
        public ICollection<Carico> Carico { get; set; }
    }
    #endregion

    #region FilesPath

    public class FilePath
    {
        public FilePath()
        {
            FilePathID = Guid.NewGuid();
        }
        [Key]
        public Guid FilePathID { get; set; }
        [StringLength(255)]
        public string FileName { get; set; }
        public FileType FileType { get; set; }
        public DateTime UploadDate { get; set; }
        public string UserID { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }

    #endregion

    #region Cartissima

    public class Cartissima
    {
        public Cartissima()
        {
            sCartId = Guid.NewGuid();
            pvID = Guid.Parse("4ced36ae-50eb-4fea-ba8e-38e2e7cf78a1");
            sCartCreateDate = DateTime.Today;
            sCartIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            sCartProcessed = false;
        }
        [Key]
        public Guid sCartId { get; set; }
        public Guid pvID { get; set; }
        public Pv Pv { get; set; }
        public DateTime sCartCreateDate { get; set; }
        public string sCartIp { get; set; }

        [Required]
        public string sCartName { get; set; }

        [Required]
        public string sCartSurname { get; set; }
        [EmailAddress]
        public string sCartEmail { get; set; }

        [Required]
        public string sCartPhone { get; set; }

        [Required]
        public string sCartCompany { get; set; }

        [Required]
        public int sCartIva { get; set; }
        public string sCartLocation { get; set; }

        [Required]
        public int sCartVeichle { get; set; }
        public string sCartVeichleType { get; set; }
        public bool sCartProcessed { get; set; }
    }

    #endregion

    #region Mail

    #endregion

    #region MyDbContext: IdentityDbContext
    public class MyDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public MyDbContext()
            : base("DefaultConnection")
        {
            /*
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
            Configuration.AutoDetectChangesEnabled = true;*/
        }

        #region Db Intializer
        /*
        static MyDbContext()
        {
            Database.SetInitializer<MyDbContext>(new MyDbInitializer());
        }
        */
        #endregion

        public static MyDbContext Create()
        {
            return new MyDbContext();
        }
        
        #region Foreign Key
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<IdentityUserLogin>().HasKey(l => l.UserId);
            modelBuilder.Entity<ApplicationUserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<IdentityRole>().HasKey(r => r.Id);
            modelBuilder.Entity<ApplicationRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
            modelBuilder.Entity<ApplicationUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<ApplicationUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<ApplicationUser>().HasRequired(p => p.UserProfiles)
                .WithMany(b => b.ApplicationUser)
                .HasForeignKey(p => p.ProfileId);
            modelBuilder.Entity<ApplicationUser>().HasRequired(p => p.Pv)
                .WithMany(b => b.ApplicationUser)
                .HasForeignKey(p => p.pvID);
            modelBuilder.Entity<ApplicationUser>().HasRequired(p => p.Company)
                .WithMany(b => b.ApplicationUser)
                .HasForeignKey(p => p.CompanyId);
            modelBuilder.Entity<UsersImage>().HasRequired(p => p.UserProfiles)
                .WithMany(b => b.UsersImage)
                .HasForeignKey(p => p.ProfileID);
            modelBuilder.Entity<Notice>().HasRequired(p => p.ApplicationUser)
                .WithMany(b => b.Notice)
                .HasForeignKey(p => p.UsersId);
            modelBuilder.Entity<UserArea>().HasRequired(p => p.ApplicationUser)
                .WithMany(b => b.UserArea)
                .HasForeignKey(p => p.UsersId);
            modelBuilder.Entity<CompanyTask>().HasRequired(p => p.ApplicationUser)
                .WithMany(b => b.CompanyTask)
                .HasForeignKey(p => p.UsersId);
            modelBuilder.Entity<FilePath>().HasRequired(p => p.ApplicationUser)
                .WithMany(b => b.FilePaths)
                .HasForeignKey(p => p.UserID);
            modelBuilder.Entity<Cartissima>().HasRequired(p => p.Pv)
                .WithMany(b => b.Cartissima)
                .HasForeignKey(p => p.pvID);
            modelBuilder.Entity<PvProfile>().HasRequired(p => p.Pv)
                .WithMany(b => b.PvProfile)
                .HasForeignKey(p => p.pvID);
        }
        #endregion

        #region Db Set
        public  DbSet<Pv> Pv { get; set; }
        public  DbSet<PvProfile> PvProfile { get; set; }
        public  DbSet<PvTank> PvTank { get; set; }
        public  DbSet<PvTankDesc> PvTankDesc { get; set; }
        public  DbSet<PvDeficienze> PvDeficienze { get; set; }
        public  DbSet<PvCali> PvCali { get; set; }
        public  DbSet<Product> Product { get; set; }
        public  DbSet<Flag> Flag { get; set; }
        public  DbSet<Carico> Carico { get; set; }
        public  DbSet<UserProfiles> UserProfiles { get; set; }
        public  DbSet<PvErogatori> PvErogatori { get; set; }
        public  DbSet<Company> Company { get; set; }
        public  DbSet<RagioneSociale> RagioneSociale { get; set; }
        public  DbSet<Dispenser> Dispenser { get; set; }
        public  DbSet<Notice> Notice { get; set; }
        public  DbSet<UserArea> UserArea { get; set; }
        public  DbSet<CompanyTask> CompanyTask { get; set; }
        public  DbSet<Year> Year { get; set; }
        public  DbSet<UsersImage> UsersImage { get; set; }
        public  DbSet<FilePath> FilePaths { get; set; }
        public  DbSet<Cartissima> Cartissima { get; set; }

        #endregion
    }
    #endregion

    #region User Store
    public class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>, IUserStore<ApplicationUser, string>, IDisposable
    {
        public ApplicationUserStore()
            : this(new IdentityDbContext())
        {
            DisposeContext = true;
        }

        public ApplicationUserStore(DbContext context)
            : base(context)
        {
        }
    }
    #endregion

    #region Role Store
    public class ApplicationRoleStore : RoleStore<ApplicationRole, string, ApplicationUserRole>, IQueryableRoleStore<ApplicationRole, string>, IRoleStore<ApplicationRole, string>, IDisposable
    {
        public ApplicationRoleStore()
            : base(new IdentityDbContext())
        {
            DisposeContext = true;
        }

        public ApplicationRoleStore(DbContext context)
            : base(context)
        {
        }
    }
    #endregion
}