﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:4.0.30319.42000
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SystemWeb.Models.LinqToSql
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="UTF")]
	public partial class SystemWebDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Definizioni metodo Extensibility
    partial void OnCreated();
    partial void InsertCarico(Carico instance);
    partial void UpdateCarico(Carico instance);
    partial void DeleteCarico(Carico instance);
    partial void InsertYear(Year instance);
    partial void UpdateYear(Year instance);
    partial void DeleteYear(Year instance);
    partial void InsertPv(Pv instance);
    partial void UpdatePv(Pv instance);
    partial void DeletePv(Pv instance);
    partial void InsertFlag(Flag instance);
    partial void UpdateFlag(Flag instance);
    partial void DeleteFlag(Flag instance);
    #endregion
		
		public SystemWebDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["UTFConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public SystemWebDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SystemWebDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SystemWebDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SystemWebDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Carico> Carico
		{
			get
			{
				return this.GetTable<Carico>();
			}
		}
		
		public System.Data.Linq.Table<Year> Year
		{
			get
			{
				return this.GetTable<Year>();
			}
		}
		
		public System.Data.Linq.Table<Pv> Pv
		{
			get
			{
				return this.GetTable<Pv>();
			}
		}
		
		public System.Data.Linq.Table<Flag> Flag
		{
			get
			{
				return this.GetTable<Flag>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Carico")]
	public partial class Carico : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _Id;
		
		private System.Guid _pvID;
		
		private int _Ordine;
		
		private System.DateTime _cData;
		
		private string _Documento;
		
		private string _Numero;
		
		private System.DateTime _rData;
		
		private string _Emittente;
		
		private int _Benzina;
		
		private int _Gasolio;
		
		private string _Note;
		
		private System.Nullable<System.Guid> _yearId;
		
		private EntityRef<Year> _Year;
		
		private EntityRef<Pv> _Pv;
		
    #region Definizioni metodo Extensibility
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(System.Guid value);
    partial void OnIdChanged();
    partial void OnpvIDChanging(System.Guid value);
    partial void OnpvIDChanged();
    partial void OnOrdineChanging(int value);
    partial void OnOrdineChanged();
    partial void OncDataChanging(System.DateTime value);
    partial void OncDataChanged();
    partial void OnDocumentoChanging(string value);
    partial void OnDocumentoChanged();
    partial void OnNumeroChanging(string value);
    partial void OnNumeroChanged();
    partial void OnrDataChanging(System.DateTime value);
    partial void OnrDataChanged();
    partial void OnEmittenteChanging(string value);
    partial void OnEmittenteChanged();
    partial void OnBenzinaChanging(int value);
    partial void OnBenzinaChanged();
    partial void OnGasolioChanging(int value);
    partial void OnGasolioChanged();
    partial void OnNoteChanging(string value);
    partial void OnNoteChanged();
    partial void OnyearIdChanging(System.Nullable<System.Guid> value);
    partial void OnyearIdChanged();
    #endregion
		
		public Carico()
		{
			this._Year = default(EntityRef<Year>);
			this._Pv = default(EntityRef<Pv>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_pvID", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid pvID
		{
			get
			{
				return this._pvID;
			}
			set
			{
				if ((this._pvID != value))
				{
					if (this._Pv.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnpvIDChanging(value);
					this.SendPropertyChanging();
					this._pvID = value;
					this.SendPropertyChanged("pvID");
					this.OnpvIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Ordine", DbType="Int NOT NULL")]
		public int Ordine
		{
			get
			{
				return this._Ordine;
			}
			set
			{
				if ((this._Ordine != value))
				{
					this.OnOrdineChanging(value);
					this.SendPropertyChanging();
					this._Ordine = value;
					this.SendPropertyChanged("Ordine");
					this.OnOrdineChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_cData", DbType="DateTime NOT NULL")]
		public System.DateTime cData
		{
			get
			{
				return this._cData;
			}
			set
			{
				if ((this._cData != value))
				{
					this.OncDataChanging(value);
					this.SendPropertyChanging();
					this._cData = value;
					this.SendPropertyChanged("cData");
					this.OncDataChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Documento", DbType="NVarChar(3)")]
		public string Documento
		{
			get
			{
				return this._Documento;
			}
			set
			{
				if ((this._Documento != value))
				{
					this.OnDocumentoChanging(value);
					this.SendPropertyChanging();
					this._Documento = value;
					this.SendPropertyChanged("Documento");
					this.OnDocumentoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Numero", DbType="NVarChar(12)")]
		public string Numero
		{
			get
			{
				return this._Numero;
			}
			set
			{
				if ((this._Numero != value))
				{
					this.OnNumeroChanging(value);
					this.SendPropertyChanging();
					this._Numero = value;
					this.SendPropertyChanged("Numero");
					this.OnNumeroChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_rData", DbType="DateTime NOT NULL")]
		public System.DateTime rData
		{
			get
			{
				return this._rData;
			}
			set
			{
				if ((this._rData != value))
				{
					this.OnrDataChanging(value);
					this.SendPropertyChanging();
					this._rData = value;
					this.SendPropertyChanged("rData");
					this.OnrDataChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Emittente", DbType="NVarChar(42)")]
		public string Emittente
		{
			get
			{
				return this._Emittente;
			}
			set
			{
				if ((this._Emittente != value))
				{
					this.OnEmittenteChanging(value);
					this.SendPropertyChanging();
					this._Emittente = value;
					this.SendPropertyChanged("Emittente");
					this.OnEmittenteChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Benzina", DbType="Int NOT NULL")]
		public int Benzina
		{
			get
			{
				return this._Benzina;
			}
			set
			{
				if ((this._Benzina != value))
				{
					this.OnBenzinaChanging(value);
					this.SendPropertyChanging();
					this._Benzina = value;
					this.SendPropertyChanged("Benzina");
					this.OnBenzinaChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Gasolio", DbType="Int NOT NULL")]
		public int Gasolio
		{
			get
			{
				return this._Gasolio;
			}
			set
			{
				if ((this._Gasolio != value))
				{
					this.OnGasolioChanging(value);
					this.SendPropertyChanging();
					this._Gasolio = value;
					this.SendPropertyChanged("Gasolio");
					this.OnGasolioChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Note", DbType="NVarChar(MAX)")]
		public string Note
		{
			get
			{
				return this._Note;
			}
			set
			{
				if ((this._Note != value))
				{
					this.OnNoteChanging(value);
					this.SendPropertyChanging();
					this._Note = value;
					this.SendPropertyChanged("Note");
					this.OnNoteChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_yearId", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> yearId
		{
			get
			{
				return this._yearId;
			}
			set
			{
				if ((this._yearId != value))
				{
					if (this._Year.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnyearIdChanging(value);
					this.SendPropertyChanging();
					this._yearId = value;
					this.SendPropertyChanged("yearId");
					this.OnyearIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Year_Carico", Storage="_Year", ThisKey="yearId", OtherKey="yearId", IsForeignKey=true)]
		public Year Year
		{
			get
			{
				return this._Year.Entity;
			}
			set
			{
				Year previousValue = this._Year.Entity;
				if (((previousValue != value) 
							|| (this._Year.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Year.Entity = null;
						previousValue.Carico.Remove(this);
					}
					this._Year.Entity = value;
					if ((value != null))
					{
						value.Carico.Add(this);
						this._yearId = value.yearId;
					}
					else
					{
						this._yearId = default(Nullable<System.Guid>);
					}
					this.SendPropertyChanged("Year");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Pv_Carico", Storage="_Pv", ThisKey="pvID", OtherKey="pvID", IsForeignKey=true, DeleteOnNull=true, DeleteRule="CASCADE")]
		public Pv Pv
		{
			get
			{
				return this._Pv.Entity;
			}
			set
			{
				Pv previousValue = this._Pv.Entity;
				if (((previousValue != value) 
							|| (this._Pv.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Pv.Entity = null;
						previousValue.Carico.Remove(this);
					}
					this._Pv.Entity = value;
					if ((value != null))
					{
						value.Carico.Add(this);
						this._pvID = value.pvID;
					}
					else
					{
						this._pvID = default(System.Guid);
					}
					this.SendPropertyChanged("Pv");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Year")]
	public partial class Year : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _yearId;
		
		private System.DateTime _Anno;
		
		private EntitySet<Carico> _Carico;
		
    #region Definizioni metodo Extensibility
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnyearIdChanging(System.Guid value);
    partial void OnyearIdChanged();
    partial void OnAnnoChanging(System.DateTime value);
    partial void OnAnnoChanged();
    #endregion
		
		public Year()
		{
			this._Carico = new EntitySet<Carico>(new Action<Carico>(this.attach_Carico), new Action<Carico>(this.detach_Carico));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_yearId", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid yearId
		{
			get
			{
				return this._yearId;
			}
			set
			{
				if ((this._yearId != value))
				{
					this.OnyearIdChanging(value);
					this.SendPropertyChanging();
					this._yearId = value;
					this.SendPropertyChanged("yearId");
					this.OnyearIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Anno", DbType="DateTime NOT NULL")]
		public System.DateTime Anno
		{
			get
			{
				return this._Anno;
			}
			set
			{
				if ((this._Anno != value))
				{
					this.OnAnnoChanging(value);
					this.SendPropertyChanging();
					this._Anno = value;
					this.SendPropertyChanged("Anno");
					this.OnAnnoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Year_Carico", Storage="_Carico", ThisKey="yearId", OtherKey="yearId")]
		public EntitySet<Carico> Carico
		{
			get
			{
				return this._Carico;
			}
			set
			{
				this._Carico.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Carico(Carico entity)
		{
			this.SendPropertyChanging();
			entity.Year = this;
		}
		
		private void detach_Carico(Carico entity)
		{
			this.SendPropertyChanging();
			entity.Year = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Pv")]
	public partial class Pv : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _pvID;
		
		private string _pvName;
		
		private System.Nullable<System.Guid> _pvFlagId;
		
		private EntitySet<Carico> _Carico;
		
		private EntityRef<Flag> _Flag;
		
    #region Definizioni metodo Extensibility
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnpvIDChanging(System.Guid value);
    partial void OnpvIDChanged();
    partial void OnpvNameChanging(string value);
    partial void OnpvNameChanged();
    partial void OnpvFlagIdChanging(System.Nullable<System.Guid> value);
    partial void OnpvFlagIdChanged();
    #endregion
		
		public Pv()
		{
			this._Carico = new EntitySet<Carico>(new Action<Carico>(this.attach_Carico), new Action<Carico>(this.detach_Carico));
			this._Flag = default(EntityRef<Flag>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_pvID", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid pvID
		{
			get
			{
				return this._pvID;
			}
			set
			{
				if ((this._pvID != value))
				{
					this.OnpvIDChanging(value);
					this.SendPropertyChanging();
					this._pvID = value;
					this.SendPropertyChanged("pvID");
					this.OnpvIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_pvName", DbType="NVarChar(4)")]
		public string pvName
		{
			get
			{
				return this._pvName;
			}
			set
			{
				if ((this._pvName != value))
				{
					this.OnpvNameChanging(value);
					this.SendPropertyChanging();
					this._pvName = value;
					this.SendPropertyChanged("pvName");
					this.OnpvNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_pvFlagId", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> pvFlagId
		{
			get
			{
				return this._pvFlagId;
			}
			set
			{
				if ((this._pvFlagId != value))
				{
					if (this._Flag.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnpvFlagIdChanging(value);
					this.SendPropertyChanging();
					this._pvFlagId = value;
					this.SendPropertyChanged("pvFlagId");
					this.OnpvFlagIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Pv_Carico", Storage="_Carico", ThisKey="pvID", OtherKey="pvID")]
		public EntitySet<Carico> Carico
		{
			get
			{
				return this._Carico;
			}
			set
			{
				this._Carico.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Flag_Pv", Storage="_Flag", ThisKey="pvFlagId", OtherKey="pvFlagId", IsForeignKey=true)]
		public Flag Flag
		{
			get
			{
				return this._Flag.Entity;
			}
			set
			{
				Flag previousValue = this._Flag.Entity;
				if (((previousValue != value) 
							|| (this._Flag.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Flag.Entity = null;
						previousValue.Pv.Remove(this);
					}
					this._Flag.Entity = value;
					if ((value != null))
					{
						value.Pv.Add(this);
						this._pvFlagId = value.pvFlagId;
					}
					else
					{
						this._pvFlagId = default(Nullable<System.Guid>);
					}
					this.SendPropertyChanged("Flag");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Carico(Carico entity)
		{
			this.SendPropertyChanging();
			entity.Pv = this;
		}
		
		private void detach_Carico(Carico entity)
		{
			this.SendPropertyChanging();
			entity.Pv = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Flag")]
	public partial class Flag : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _pvFlagId;
		
		private string _Nome;
		
		private string _Descrizione;
		
		private EntitySet<Pv> _Pv;
		
    #region Definizioni metodo Extensibility
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnpvFlagIdChanging(System.Guid value);
    partial void OnpvFlagIdChanged();
    partial void OnNomeChanging(string value);
    partial void OnNomeChanged();
    partial void OnDescrizioneChanging(string value);
    partial void OnDescrizioneChanged();
    #endregion
		
		public Flag()
		{
			this._Pv = new EntitySet<Pv>(new Action<Pv>(this.attach_Pv), new Action<Pv>(this.detach_Pv));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_pvFlagId", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid pvFlagId
		{
			get
			{
				return this._pvFlagId;
			}
			set
			{
				if ((this._pvFlagId != value))
				{
					this.OnpvFlagIdChanging(value);
					this.SendPropertyChanging();
					this._pvFlagId = value;
					this.SendPropertyChanged("pvFlagId");
					this.OnpvFlagIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Nome", DbType="NVarChar(12)")]
		public string Nome
		{
			get
			{
				return this._Nome;
			}
			set
			{
				if ((this._Nome != value))
				{
					this.OnNomeChanging(value);
					this.SendPropertyChanging();
					this._Nome = value;
					this.SendPropertyChanged("Nome");
					this.OnNomeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Descrizione", DbType="NVarChar(MAX)")]
		public string Descrizione
		{
			get
			{
				return this._Descrizione;
			}
			set
			{
				if ((this._Descrizione != value))
				{
					this.OnDescrizioneChanging(value);
					this.SendPropertyChanging();
					this._Descrizione = value;
					this.SendPropertyChanged("Descrizione");
					this.OnDescrizioneChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Flag_Pv", Storage="_Pv", ThisKey="pvFlagId", OtherKey="pvFlagId")]
		public EntitySet<Pv> Pv
		{
			get
			{
				return this._Pv;
			}
			set
			{
				this._Pv.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Pv(Pv entity)
		{
			this.SendPropertyChanging();
			entity.Flag = this;
		}
		
		private void detach_Pv(Pv entity)
		{
			this.SendPropertyChanging();
			entity.Flag = null;
		}
	}
}
#pragma warning restore 1591
