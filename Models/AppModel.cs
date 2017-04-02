
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
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true)]
    public class ApplicationUser : IdentityUser<string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public Guid ProfileId { get; set; }
        [DataMember]
        public virtual UserProfiles UserProfiles { get; set; }
        [DataMember]
        public virtual Guid? pvID { get; set; }
        [DataMember]
        public virtual Pv Pv { get; set; }
        [DataMember]
        public virtual Guid? CompanyId { get; set; }
        public DateTime CreateDate { get; set; }
        [DataMember]
        public virtual Company Company { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
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
        [JsonIgnore]
        public ICollection<Notice> Notice { get; set; }
        [JsonIgnore]
        public ICollection<UserArea> UserArea { get; set; }
        [JsonIgnore]
        public ICollection<CompanyTask> CompanyTask { get; set; }

        public virtual ICollection<FilePath> FilePaths { get; set; }
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
        public virtual UserProfiles UserProfiles { get; set; }
    }
    #endregion

    #region Carico
    [DataContract(IsReference = true)]
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
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yy}", ApplyFormatInEditMode = true)]
        public DateTime cData { get; set; }
        [MaxLength(4)]
        public string Documento { get; set; }
        [MaxLength(18)]
        public string Numero { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yy}", ApplyFormatInEditMode = true)]
        public System.DateTime rData { get; set; }
        [MaxLength(32)]
        public string Emittente { get; set; }
        public int Benzina { get; set; }
        public int Gasolio { get; set; }
        public string Note { get; set; }
        [DataMember]
        public virtual Pv Pv { get; set; }
        [DataMember]
        public virtual Year Year { get; set; }

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
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true)]
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
        [DataMember]
        public virtual Guid? RagioneSocialeId { get; set; }
        [DataMember]
        public virtual RagioneSociale RagioneSociale { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }
    }
    #endregion

    #region Company Task
    [DataContract(IsReference = true)]
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
        [DataMember]
        public virtual ApplicationUser ApplicationUser { get; set; }
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
    [DataContract(IsReference = true)]
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
        [DataMember]
        public virtual Product Product { get; set; }
        [DataMember]
        public virtual Dispenser Dispenser { get; set; }
        [DataMember]
        public virtual Pv Pv { get; set; }
        [NotMapped]
        public string sspb { get; set; }
        [NotMapped]
        public string dsl { get; set; }
    }
    #endregion

    #region Flag
    [JsonObject(IsReference = true)]
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
        [JsonIgnore]
        public virtual ICollection<Pv> Pv { get; set; }
    }
    #endregion

    #region Product
    [JsonObject(IsReference = true)]
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
        [JsonIgnore]
        public virtual ICollection<PvTank> PvTank { get; set; }
    }
    #endregion

    #region Pv
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true)]
    public class Pv
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pv()
        {
            PvTank = new HashSet<PvTank>();
            Carico = new HashSet<Carico>();
            PvProfile = new HashSet<PvProfile>();
            ApplicationUser = new HashSet<ApplicationUser>();
            pvID = Guid.NewGuid();
        }

        [Key]
        public Guid pvID { get; set; }
        [MaxLength(8)]
        public string pvName { get; set; }
        public Guid? pvFlagId { get; set; }
        [DataMember]
        public virtual Flag Flag { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PvTank> PvTank { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<Carico> Carico { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PvProfile> PvProfile { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }
    }
    #endregion

    #region Pv Profile
    [DataContract(IsReference = true)]
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
        [DataMember]
        public virtual Pv Pv { get; set; }
    }
    #endregion

    #region Pv Tank
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true)]
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
        [DataMember]
        public virtual Product Product { get; set; }
        [DataMember]
        public virtual Pv Pv { get; set; }
        [JsonIgnore]
        public virtual ICollection<PvTankDesc> PvTankDesc { get; set; }
        [JsonIgnore]
        public virtual ICollection<PvDeficienze> PvDeficienze { get; set; }
        [JsonIgnore]
        public virtual ICollection<PvCali> PvCali { get; set; }
    }
    #endregion

    #region Pv Deficienze
    [DataContract(IsReference = true)]
    public class PvDeficienze
    {
        public PvDeficienze()
        {
            PvDefId = Guid.NewGuid();
        }

        [Key]
        public Guid PvDefId { get; set; }
        public Guid PvTankId { get; set; }
        [DataMember]
        public virtual PvTank PvTank { get; set; }
        public int Value { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yy}", ApplyFormatInEditMode = true)]
        public DateTime FieldDate { get; set; }
    }
    #endregion

    #region Pv Cali
    [DataContract(IsReference = true)]
    public class PvCali
    {
        public PvCali()
        {
            PvCaliId = Guid.NewGuid();
        }

        [Key]
        public Guid PvCaliId { get; set; }
        public Guid PvTankId { get; set; }
        [DataMember]
        public virtual PvTank PvTank { get; set; }
        public int Value { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yy}", ApplyFormatInEditMode = true)]
        public DateTime FieldDate { get; set; }
    }
    #endregion

    #region Pv Tank Desc
    [DataContract(IsReference = true)]
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
        [DataMember]
        public virtual PvTank PvTank { get; set; }
    }
    #endregion

    #region User Profile
    [JsonObject(IsReference = true)]
    public class UserProfiles
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
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

        [JsonIgnore]
        public ICollection<ApplicationUser> ApplicationUser { get; set; }
        public ICollection<UsersImage> UsersImage { get; set; }
    }
    #endregion

    #region Dispenser
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true)]
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
        [DataMember]
        public virtual PvTank PvTank { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PvErogatori> PvErogatori { get; set; }
        public bool? isActive { get; set; }
    }
    #endregion

    #region Notice
    [DataContract(IsReference = true)]
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
        [DataMember]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public override string ToString()
        {
            return string.Format(
                "NoticeName: {0}, CreateDate: {1}, TextBox: {2}, UsersId: {3}, Description: {4}", NoticeName, CreateDate, TextBox, UsersId, Description);
        }

    }
    #endregion

    #region User Area
    [DataContract(IsReference = true)]
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
        [DataMember]
        public virtual ApplicationUser ApplicationUser { get; set; }
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
    [JsonObject(IsReference = true)]
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<Carico> Carico { get; set; }
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
        public virtual ApplicationUser ApplicationUser { get; set; }
    }

    #endregion

    #region MyDbContext: IdentityDbContext
    public class MyDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public MyDbContext()
            : base("DefaultConnection")
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
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
            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<ApplicationUserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
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
        }
        #endregion

        #region Db Set

        public virtual DbSet<Pv> Pv { get; set; }
        public virtual DbSet<PvProfile> PvProfile { get; set; }
        public virtual DbSet<PvTank> PvTank { get; set; }
        public virtual DbSet<PvTankDesc> PvTankDesc { get; set; }
        public virtual DbSet<PvDeficienze> PvDeficienze { get; set; }
        public virtual DbSet<PvCali> PvCali { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Flag> Flag { get; set; }
        public virtual DbSet<Carico> Carico { get; set; }
        public virtual DbSet<UserProfiles> UserProfiles { get; set; }
        public virtual DbSet<PvErogatori> PvErogatori { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<RagioneSociale> RagioneSociale { get; set; }
        public virtual DbSet<Dispenser> Dispenser { get; set; }
        public virtual DbSet<Notice> Notice { get; set; }
        public virtual DbSet<UserArea> UserArea { get; set; }
        public virtual DbSet<CompanyTask> CompanyTask { get; set; }
        public virtual DbSet<Year> Year { get; set; }
        public virtual DbSet<UsersImage> UsersImage { get; set; }
        public virtual DbSet<FilePath> FilePaths { get; set; }

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