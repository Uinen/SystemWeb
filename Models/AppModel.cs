using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SystemWeb.Models
{
    public class ApplicationUserLogin : IdentityUserLogin<string> { }
    public class ApplicationUserClaim : IdentityUserClaim<string> { }
    public class ApplicationUserRole : IdentityUserRole<string> 
    {

    }

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
            this.Id = Guid.NewGuid().ToString();
            this.Notice = new HashSet<Notice>();
            this.UserArea = new HashSet<UserArea>();
            this.CompanyTask = new HashSet<CompanyTask>();
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
    }
    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    {
        public ApplicationRole()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public ApplicationRole(string name)
            : this()
        {
            this.Name = name;
        }
    }

    [DataContract(IsReference = true)]
    public class Carico
    {
        public Carico()
        {
            this.Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        public Guid pvID { get; set; }
        public Nullable <System.Guid> yearId { get; set; }
        public int Ordine { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yy}", ApplyFormatInEditMode = true)]
        public System.DateTime cData { get; set; }
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
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true)]
    public class Company
    {
        public Company()
        {
            this.CompanyId = Guid.NewGuid();
            this.ApplicationUser = new HashSet<ApplicationUser>();
        }
        [Key]
        public Guid CompanyId { get; set; }
        [MaxLength(32)]
        public string Name { get; set; }
        public int PartitaIva { get; set; }
        [DataMember]
        public virtual Nullable <Guid> RagioneSocialeId { get; set; }
        [DataMember]
        public virtual RagioneSociale RagioneSociale { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }
    }

    [DataContract(IsReference = true)]
    public class CompanyTask
    {
        public CompanyTask()
        {
            this.CompanyTaskId = Guid.NewGuid();
        }
        [Key]
        public Guid CompanyTaskId { get; set; }
        [MaxLength(128)]
        public string UsersId { get; set; }
        public string FieldChiusura { get; set;}
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yy}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> FieldDate { get; set; }
        public float FieldResult { get; set; }
        [DataMember]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }

    public class RagioneSociale
    {
        public RagioneSociale()
        {
            this.RagioneSocialeId = Guid.NewGuid();
        }
        [Key]
        public Guid RagioneSocialeId { get; set; }
        [MaxLength(10)]
        public string Nome { get; set; }
    }

    [DataContract(IsReference = true)]
    public class PvErogatori
    {
        public PvErogatori()
        {
            this.PvErogatoriId = Guid.NewGuid();
        }
        [Key]
        public Guid PvErogatoriId { get; set; }
        public System.Guid pvID { get; set; }
        public System.Guid ProductId { get; set; }
        public System.Guid DispenserId { get; set; }
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

    [JsonObject(IsReference = true)]
    public class Flag
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Flag()
        {
            this.Pv = new HashSet<Pv>();
            this.pvFlagId = Guid.NewGuid();
        }
        [Key]
        public System.Guid pvFlagId { get; set; }
        [MaxLength(10)]
        public string Nome { get; set; }
        [MaxLength(64)]
        public string Descrizione { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<Pv> Pv { get; set; }
    }

    [JsonObject(IsReference = true)]
    public class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.PvTank = new HashSet<PvTank>();
            this.ProductId = Guid.NewGuid();
        }
        [Key]
        public System.Guid ProductId { get; set; }
        [MaxLength(14)]
        public string Nome { get; set; }
        public float Peso { get; set; }
        public float Prezzo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PvTank> PvTank { get; set; }
    }

    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true)]
    public class Pv
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pv()
        {
            this.PvTank = new HashSet<PvTank>();
            this.Carico = new HashSet<Carico>();
            this.PvProfile = new HashSet<PvProfile>();
            this.ApplicationUser = new HashSet<ApplicationUser>();
            this.pvID = Guid.NewGuid();
        }

        [Key]
        public System.Guid pvID { get; set; }
        [MaxLength(8)]
        public string pvName { get; set; }
        public Nullable <System.Guid> pvFlagId { get; set; }
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
        public virtual ICollection <ApplicationUser> ApplicationUser { get; set; }
    }

    [DataContract(IsReference = true)]
    public class PvProfile
    {
        public PvProfile()
        {
            this.PvProfileId = Guid.NewGuid();
        }
        [Key]
        public System.Guid PvProfileId { get; set; }
        public System.Guid pvID { get; set; }
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

    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true)]
    public class PvTank
    {
        public PvTank()
        {
            this.PvTankId = Guid.NewGuid();
            this.PvTankDesc = new HashSet<PvTankDesc>();
            this.PvDeficienze = new HashSet<PvDeficienze>();
            this.PvCali = new HashSet<PvCali>();
        }
        [Key]
        public System.Guid PvTankId { get; set; }
        public Guid pvID { get; set; }
        public System.Guid ProductId { get; set; }
        [MaxLength(14)]
        public string Modello { get; set; }
        public System.DateTime LastDate { get; set; }
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

    [DataContract(IsReference = true)]
    public class PvDeficienze
    {
        public PvDeficienze()
        {
            this.PvDefId = Guid.NewGuid();
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

    [DataContract(IsReference = true)]
    public class PvCali
    {
        public PvCali()
        {
            this.PvCaliId = Guid.NewGuid();
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

    [DataContract(IsReference = true)]
    public class PvTankDesc
    {
        public PvTankDesc()
        {
            this.PvTankDescId = Guid.NewGuid();
        }

        [Key]
        public System.Guid PvTankDescId { get; set; }
        public System.Guid PvTankId { get; set; }
        public float PvTankCM { get; set; }
        public float PvTankLT { get; set; }
        [DataMember]
        public virtual PvTank PvTank { get; set; }
    }

    [JsonObject(IsReference = true)]
    public class UserProfiles
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserProfiles()
        {
            this.ProfileId = Guid.NewGuid();
        }
        [Key]
        public Guid ProfileId { get; set; }
        [MaxLength(14)]
        public string ProfileName { get; set; }
        [MaxLength(14)]
        public string ProfileSurname { get; set; }
        [MaxLength(32)]
        public string ProfileAdress { get; set; }
        [MaxLength(14)]
        public string ProfileCity { get; set; }
        public int ProfileZipCode { get; set; }
        [MaxLength(14)]
        public string ProfileNation { get; set; }
        public string ProfileInfo { get; set; }
        [JsonIgnore]
        public ICollection<ApplicationUser> ApplicationUser { get; set; }
    }

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

    [DataContract(IsReference = true)]
    public class Notice
    {
        public Notice ()
        {
            this.NoticeId = Guid.NewGuid();
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

    [DataContract(IsReference = true)]
    public class UserArea
    {
        public UserArea ()
        {
            this.UserAreaId = Guid.NewGuid();
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

    [JsonObject(IsReference = true)]
    public class Year
    {
        public Year ()
        {
            this.yearId = Guid.NewGuid();
            this.Carico = new HashSet<Carico>();
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

    public class MyDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public MyDbContext()
            : base("DefaultConnection")
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true; 
        }
        /*
        static MyDbContext()
        {
            Database.SetInitializer<MyDbContext>(new MyDbInitializer());
        }
        */
        public static MyDbContext Create()
        {
            return new MyDbContext();
        }
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
            modelBuilder.Entity<Notice>().HasRequired(p => p.ApplicationUser)
                .WithMany(b => b.Notice)
                .HasForeignKey(p => p.UsersId);
            modelBuilder.Entity<UserArea>().HasRequired(p => p.ApplicationUser)
                .WithMany(b => b.UserArea)
                .HasForeignKey(p => p.UsersId);
            modelBuilder.Entity<CompanyTask>().HasRequired(p => p.ApplicationUser)
                .WithMany(b => b.CompanyTask)
                .HasForeignKey(p => p.UsersId);
        }
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
    }
    public class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>, IUserStore<ApplicationUser, string>, IDisposable
    {
        public ApplicationUserStore()
            : this(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationUserStore(DbContext context)
            : base(context)
        {
        }
    }
    public class ApplicationRoleStore : RoleStore<ApplicationRole, string, ApplicationUserRole>, IQueryableRoleStore<ApplicationRole, string>, IRoleStore<ApplicationRole, string>, IDisposable
    {
        public ApplicationRoleStore()
            : base(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationRoleStore(DbContext context)
            : base(context)
        {
        }
    }
}