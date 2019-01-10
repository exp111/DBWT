//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/linq2db).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Data;
using LinqToDB.Mapping;

namespace DbModels
{
	/// <summary>
	/// Database       : dbwt
	/// Data Source    : localhost
	/// Server Version : 5.5.5-10.1.31-MariaDB
	/// </summary>
	public partial class DbwtDB : LinqToDB.Data.DataConnection
	{
		public ITable<Benutzer>                         Benutzer                         { get { return this.GetTable<Benutzer>(); } }
		public ITable<Bestellungen>                     Bestellungen                     { get { return this.GetTable<Bestellungen>(); } }
		public ITable<Bestellungenenthältmahlzeiten>    Bestellungenenthältmahlzeiten    { get { return this.GetTable<Bestellungenenthältmahlzeiten>(); } }
		public ITable<Bilder>                           Bilder                           { get { return this.GetTable<Bilder>(); } }
		public ITable<Deklarationen>                    Deklarationen                    { get { return this.GetTable<Deklarationen>(); } }
		public ITable<Fachbereiche>                     Fachbereiche                     { get { return this.GetTable<Fachbereiche>(); } }
		public ITable<FhAngehörige>                     FhAngehörige                     { get { return this.GetTable<FhAngehörige>(); } }
		public ITable<Fhangehörigergehörtzufachbereich> Fhangehörigergehörtzufachbereich { get { return this.GetTable<Fhangehörigergehörtzufachbereich>(); } }
		public ITable<Freundesliste>                    Freundesliste                    { get { return this.GetTable<Freundesliste>(); } }
		public ITable<Gäste>                            Gäste                            { get { return this.GetTable<Gäste>(); } }
		public ITable<Kategorien>                       Kategorien                       { get { return this.GetTable<Kategorien>(); } }
		public ITable<Kommentare>                       Kommentare                       { get { return this.GetTable<Kommentare>(); } }
		public ITable<Mahlzeitbrauchtdeklaration>       Mahlzeitbrauchtdeklaration       { get { return this.GetTable<Mahlzeitbrauchtdeklaration>(); } }
		public ITable<Mahlzeiten>                       Mahlzeiten                       { get { return this.GetTable<Mahlzeiten>(); } }
		public ITable<Mahlzeitenthältzutat>             Mahlzeitenthältzutat             { get { return this.GetTable<Mahlzeitenthältzutat>(); } }
		public ITable<Mahlzeithatbilder>                Mahlzeithatbilder                { get { return this.GetTable<Mahlzeithatbilder>(); } }
		public ITable<Mitarbeiter>                      Mitarbeiter                      { get { return this.GetTable<Mitarbeiter>(); } }
		public ITable<Preise>                           Preise                           { get { return this.GetTable<Preise>(); } }
		public ITable<Produkte>                         Produkte                         { get { return this.GetTable<Produkte>(); } }
		public ITable<Studenten>                        Studenten                        { get { return this.GetTable<Studenten>(); } }
		public ITable<Zutaten>                          Zutaten                          { get { return this.GetTable<Zutaten>(); } }

		public void InitMappingSchema()
		{
		}

		public DbwtDB()
		{
			InitDataContext();
			InitMappingSchema();
		}

		public DbwtDB(string configuration)
			: base(configuration)
		{
			InitDataContext();
			InitMappingSchema();
		}

		partial void InitDataContext();
	}

	[Table("benutzer")]
	public partial class Benutzer
	{
		[Column(),                PrimaryKey,  Identity] public int       Nummer       { get; set; } // int(11)
		[Column("E-Mail"),        NotNull              ] public string    EMail        { get; set; } // varchar(255)
		[Column(),                NotNull              ] public string    Nutzername   { get; set; } // varchar(255)
		[Column("Letzter Login"), NotNull              ] public DateTime  LetzterLogin { get; set; } // timestamp
		[Column(),                   Nullable          ] public DateTime? Geburtsdatum { get; set; } // date
		[Column(),                   Nullable          ] public int?      Age          { get; set; } // int(11)
		[Column(),                NotNull              ] public DateTime  Anlegedatum  { get; set; } // date
		[Column(),                NotNull              ] public bool      Aktiv        { get; set; } // tinyint(1)
		[Column(),                NotNull              ] public string    Vorname      { get; set; } // varchar(50)
		[Column(),                NotNull              ] public string    Nachname     { get; set; } // varchar(50)
		[Column(),                NotNull              ] public string    Salt         { get; set; } // char(32)
		[Column(),                NotNull              ] public string    Hash         { get; set; } // char(24)

		#region Associations

		/// <summary>
		/// FHIstEinBenutzer_BackReference
		/// </summary>
		[Association(ThisKey="Nummer", OtherKey="Nummer", CanBeNull=true, Relationship=Relationship.OneToOne, IsBackReference=true)]
		public FhAngehörige FHIstEinBenutzer { get; set; }

		/// <summary>
		/// freundesliste_ibfk_2_BackReference
		/// </summary>
		[Association(ThisKey="Nummer", OtherKey="Freund", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Freundesliste> FreundeslisteIbfk2BackReferences { get; set; }

		/// <summary>
		/// freundesliste_ibfk_1_BackReference
		/// </summary>
		[Association(ThisKey="Nummer", OtherKey="User", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Freundesliste> Freundeslisteibfks { get; set; }

		/// <summary>
		/// gäste_ibfk_1_BackReference
		/// </summary>
		[Association(ThisKey="Nummer", OtherKey="Nummer", CanBeNull=true, Relationship=Relationship.OneToOne, IsBackReference=true)]
		public Gäste Gästeibfk { get; set; }

		/// <summary>
		/// GetätigtVonBenutzer_BackReference
		/// </summary>
		[Association(ThisKey="Nummer", OtherKey="GetätigtVon", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Bestellungen> GetätigtVonBenutzers { get; set; }

		#endregion
	}

	[Table("bestellungen")]
	public partial class Bestellungen
	{
		[PrimaryKey, Identity   ] public int      Nummer           { get; set; } // int(11)
		[Column,     NotNull    ] public DateTime Bestellzeitpunkt { get; set; } // timestamp
		[Column,     NotNull    ] public DateTime Abholzeitpunkt   { get; set; } // timestamp
		[Column,        Nullable] public double?  Endpreis         { get; set; } // double
		[Column,     NotNull    ] public int      GetätigtVon      { get; set; } // int(11)

		#region Associations

		/// <summary>
		/// bestellungenenthältmahlzeiten_ibfk_1_BackReference
		/// </summary>
		[Association(ThisKey="Nummer", OtherKey="Bestellung", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Bestellungenenthältmahlzeiten> Bestellungenenthältmahlzeitenibfk { get; set; }

		/// <summary>
		/// GetätigtVonBenutzer
		/// </summary>
		[Association(ThisKey="GetätigtVon", OtherKey="Nummer", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="GetätigtVonBenutzer", BackReferenceName="GetätigtVonBenutzers")]
		public Benutzer GetätigtVonBenutzer { get; set; }

		#endregion
	}

	[Table("bestellungenenthältmahlzeiten")]
	public partial class Bestellungenenthältmahlzeiten
	{
		[Column, NotNull] public int Anzahl     { get; set; } // int(11)
		[Column, NotNull] public int Bestellung { get; set; } // int(11)
		[Column, NotNull] public int Mahlzeit   { get; set; } // int(11)

		#region Associations

		/// <summary>
		/// bestellungenenthältmahlzeiten_ibfk_2
		/// </summary>
		[Association(ThisKey="Mahlzeit", OtherKey="ID", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="bestellungenenthältmahlzeiten_ibfk_2", BackReferenceName="Bestellungenenthältmahlzeitenibfk")]
		public Mahlzeiten BestellungenenthältmahlzeitenIbfk2 { get; set; }

		/// <summary>
		/// bestellungenenthältmahlzeiten_ibfk_1
		/// </summary>
		[Association(ThisKey="Bestellung", OtherKey="Nummer", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="bestellungenenthältmahlzeiten_ibfk_1", BackReferenceName="Bestellungenenthältmahlzeitenibfk")]
		public Bestellungen Ibfk { get; set; }

		#endregion
	}

	[Table("bilder")]
	public partial class Bilder
	{
		[Column(),           PrimaryKey,  Identity] public int    ID         { get; set; } // int(11)
		[Column("Alt-Text"), NotNull              ] public string AltText    { get; set; } // varchar(255)
		[Column(),              Nullable          ] public string Titel      { get; set; } // varchar(50)
		[Column(),           NotNull              ] public byte[] Binärdaten { get; set; } // mediumblob

		#region Associations

		/// <summary>
		/// kategorien_ibfk_2_BackReference
		/// </summary>
		[Association(ThisKey="ID", OtherKey="HatBilder", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Kategorien> Kategorienibfks { get; set; }

		/// <summary>
		/// mahlzeithatbilder_ibfk_2_BackReference
		/// </summary>
		[Association(ThisKey="ID", OtherKey="Bild", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Mahlzeithatbilder> Mahlzeithatbilderibfks { get; set; }

		#endregion
	}

	[Table("deklarationen")]
	public partial class Deklarationen
	{
		[PrimaryKey, NotNull] public string Zeichen      { get; set; } // varchar(2)
		[Column,     NotNull] public string Beschriftung { get; set; } // varchar(32)

		#region Associations

		/// <summary>
		/// mahlzeitbrauchtdeklaration_ibfk_2_BackReference
		/// </summary>
		[Association(ThisKey="Zeichen", OtherKey="Deklaration", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Mahlzeitbrauchtdeklaration> Mahlzeitbrauchtdeklarationibfks { get; set; }

		#endregion
	}

	[Table("fachbereiche")]
	public partial class Fachbereiche
	{
		[PrimaryKey, Identity] public int    ID      { get; set; } // int(11)
		[Column,     NotNull ] public string Name    { get; set; } // varchar(100)
		[Column,     NotNull ] public string Website { get; set; } // varchar(255)
		[Column,     NotNull ] public string Adresse { get; set; } // varchar(255)

		#region Associations

		/// <summary>
		/// fhangehörigergehörtzufachbereich_ibfk_2_BackReference
		/// </summary>
		[Association(ThisKey="ID", OtherKey="Fachbereich", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Fhangehörigergehörtzufachbereich> Fhangehörigergehörtzufachbereichibfk { get; set; }

		#endregion
	}

	[Table("fh angehörige")]
	public partial class FhAngehörige
	{
		[PrimaryKey, NotNull] public int Nummer { get; set; } // int(11)

		#region Associations

		/// <summary>
		/// fhangehörigergehörtzufachbereich_ibfk_1_BackReference
		/// </summary>
		[Association(ThisKey="Nummer", OtherKey="FHAngehöriger", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Fhangehörigergehörtzufachbereich> Fhangehörigergehörtzufachbereichibfk { get; set; }

		/// <summary>
		/// FHIstEinBenutzer
		/// </summary>
		[Association(ThisKey="Nummer", OtherKey="Nummer", CanBeNull=false, Relationship=Relationship.OneToOne, KeyName="FHIstEinBenutzer", BackReferenceName="FHIstEinBenutzer")]
		public Benutzer FHIstEinBenutzer { get; set; }

		/// <summary>
		/// mitarbeiter_ibfk_1_BackReference
		/// </summary>
		[Association(ThisKey="Nummer", OtherKey="Nummer", CanBeNull=true, Relationship=Relationship.OneToOne, IsBackReference=true)]
		public Mitarbeiter Mitarbeiteribfk { get; set; }

		/// <summary>
		/// studenten_ibfk_1_BackReference
		/// </summary>
		[Association(ThisKey="Nummer", OtherKey="Nummer", CanBeNull=true, Relationship=Relationship.OneToOne, IsBackReference=true)]
		public Studenten Studentenibfk { get; set; }

		#endregion
	}

	[Table("fhangehörigergehörtzufachbereich")]
	public partial class Fhangehörigergehörtzufachbereich
	{
		[Column, NotNull] public int FHAngehöriger { get; set; } // int(11)
		[Column, NotNull] public int Fachbereich   { get; set; } // int(11)

		#region Associations

		/// <summary>
		/// fhangehörigergehörtzufachbereich_ibfk_2
		/// </summary>
		[Association(ThisKey="Fachbereich", OtherKey="ID", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="fhangehörigergehörtzufachbereich_ibfk_2", BackReferenceName="Fhangehörigergehörtzufachbereichibfk")]
		public Fachbereiche FhangehörigergehörtzufachbereichIbfk2 { get; set; }

		/// <summary>
		/// fhangehörigergehörtzufachbereich_ibfk_1
		/// </summary>
		[Association(ThisKey="FHAngehöriger", OtherKey="Nummer", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="fhangehörigergehörtzufachbereich_ibfk_1", BackReferenceName="Fhangehörigergehörtzufachbereichibfk")]
		public FhAngehörige Ibfk { get; set; }

		#endregion
	}

	[Table("freundesliste")]
	public partial class Freundesliste
	{
		[Column, NotNull] public int User   { get; set; } // int(11)
		[Column, NotNull] public int Freund { get; set; } // int(11)

		#region Associations

		/// <summary>
		/// freundesliste_ibfk_2
		/// </summary>
		[Association(ThisKey="Freund", OtherKey="Nummer", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="freundesliste_ibfk_2", BackReferenceName="FreundeslisteIbfk2BackReferences")]
		public Benutzer FreundeslisteIbfk2 { get; set; }

		/// <summary>
		/// freundesliste_ibfk_1
		/// </summary>
		[Association(ThisKey="User", OtherKey="Nummer", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="freundesliste_ibfk_1", BackReferenceName="Freundeslisteibfks")]
		public Benutzer Ibfk { get; set; }

		#endregion
	}

	[Table("gäste")]
	public partial class Gäste
	{
		[PrimaryKey, NotNull    ] public int       Nummer      { get; set; } // int(11)
		[Column,     NotNull    ] public string    Grund       { get; set; } // varchar(255)
		[Column,        Nullable] public DateTime? Ablaufdatum { get; set; } // date

		#region Associations

		/// <summary>
		/// gäste_ibfk_1
		/// </summary>
		[Association(ThisKey="Nummer", OtherKey="Nummer", CanBeNull=false, Relationship=Relationship.OneToOne, KeyName="gäste_ibfk_1", BackReferenceName="Gästeibfk")]
		public Benutzer Ibfk { get; set; }

		#endregion
	}

	[Table("kategorien")]
	public partial class Kategorien
	{
		[PrimaryKey, Identity   ] public int    ID          { get; set; } // int(11)
		[Column,     NotNull    ] public string Bezeichnung { get; set; } // varchar(50)
		[Column,        Nullable] public int?   HatBilder   { get; set; } // int(11)
		[Column,        Nullable] public int?   Parent      { get; set; } // int(11)

		#region Associations

		/// <summary>
		/// kategorien_ibfk_1
		/// </summary>
		[Association(ThisKey="Parent", OtherKey="ID", CanBeNull=true, Relationship=Relationship.ManyToOne, KeyName="kategorien_ibfk_1", BackReferenceName="KategorienIbfk1BackReferences")]
		public Kategorien Ibfk { get; set; }

		/// <summary>
		/// inKategorien_BackReference
		/// </summary>
		[Association(ThisKey="ID", OtherKey="InKategorie", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Mahlzeiten> InKategoriens { get; set; }

		/// <summary>
		/// kategorien_ibfk_1_BackReference
		/// </summary>
		[Association(ThisKey="ID", OtherKey="Parent", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Kategorien> KategorienIbfk1BackReferences { get; set; }

		/// <summary>
		/// kategorien_ibfk_2
		/// </summary>
		[Association(ThisKey="HatBilder", OtherKey="ID", CanBeNull=true, Relationship=Relationship.ManyToOne, KeyName="kategorien_ibfk_2", BackReferenceName="Kategorienibfks")]
		public Bilder KategorienIbfk2 { get; set; }

		#endregion
	}

	[Table("kommentare")]
	public partial class Kommentare
	{
		[Column(),     PrimaryKey,  Identity] public int    ID        { get; set; } // int(11)
		[Column(),        Nullable          ] public string Bemerkung { get; set; } // varchar(255)
		[Column(),     NotNull              ] public int    Bewertung { get; set; } // int(11)
		[Column("zu"), NotNull              ] public int    Zu        { get; set; } // int(11)
		[Column(),     NotNull              ] public int    Author    { get; set; } // int(11)

		#region Associations

		/// <summary>
		/// GeschriebenVOn
		/// </summary>
		[Association(ThisKey="Author", OtherKey="Nummer", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="GeschriebenVOn", BackReferenceName="GeschriebenVOns")]
		public Studenten GeschriebenVOn { get; set; }

		/// <summary>
		/// KommentarZuMahlzeit
		/// </summary>
		[Association(ThisKey="Zu", OtherKey="ID", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="KommentarZuMahlzeit", BackReferenceName="KommentarZuMahlzeits")]
		public Mahlzeiten KommentarZuMahlzeit { get; set; }

		#endregion
	}

	[Table("mahlzeitbrauchtdeklaration")]
	public partial class Mahlzeitbrauchtdeklaration
	{
		[Column, NotNull] public int    Mahlzeit    { get; set; } // int(11)
		[Column, NotNull] public string Deklaration { get; set; } // varchar(32)

		#region Associations

		/// <summary>
		/// mahlzeitbrauchtdeklaration_ibfk_1
		/// </summary>
		[Association(ThisKey="Mahlzeit", OtherKey="ID", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="mahlzeitbrauchtdeklaration_ibfk_1", BackReferenceName="Mahlzeitbrauchtdeklarationibfks")]
		public Mahlzeiten Ibfk { get; set; }

		/// <summary>
		/// mahlzeitbrauchtdeklaration_ibfk_2
		/// </summary>
		[Association(ThisKey="Deklaration", OtherKey="Zeichen", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="mahlzeitbrauchtdeklaration_ibfk_2", BackReferenceName="Mahlzeitbrauchtdeklarationibfks")]
		public Deklarationen MahlzeitbrauchtdeklarationIbfk2 { get; set; }

		#endregion
	}

	[Table("mahlzeiten")]
	public partial class Mahlzeiten
	{
		[Column(),              PrimaryKey,  Identity] public int    ID           { get; set; } // int(11)
		[Column(),              NotNull              ] public string Beschreibung { get; set; } // varchar(255)
		[Column(),              NotNull              ] public int    Vorrat       { get; set; } // int(11)
		[Column("verfügbar"),      Nullable          ] public bool?  Verfügbar    { get; set; } // tinyint(1)
		[Column("inKategorie"),    Nullable          ] public int?   InKategorie  { get; set; } // int(11)
		[Column(),              NotNull              ] public string Name         { get; set; } // varchar(50)

		#region Associations

		/// <summary>
		/// bestellungenenthältmahlzeiten_ibfk_2_BackReference
		/// </summary>
		[Association(ThisKey="ID", OtherKey="Mahlzeit", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Bestellungenenthältmahlzeiten> Bestellungenenthältmahlzeitenibfk { get; set; }

		/// <summary>
		/// inKategorien
		/// </summary>
		[Association(ThisKey="InKategorie", OtherKey="ID", CanBeNull=true, Relationship=Relationship.ManyToOne, KeyName="inKategorien", BackReferenceName="InKategoriens")]
		public Kategorien InKategorien { get; set; }

		/// <summary>
		/// KommentarZuMahlzeit_BackReference
		/// </summary>
		[Association(ThisKey="ID", OtherKey="Zu", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Kommentare> KommentarZuMahlzeits { get; set; }

		/// <summary>
		/// mahlzeitbrauchtdeklaration_ibfk_1_BackReference
		/// </summary>
		[Association(ThisKey="ID", OtherKey="Mahlzeit", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Mahlzeitbrauchtdeklaration> Mahlzeitbrauchtdeklarationibfks { get; set; }

		/// <summary>
		/// mahlzeitenthältzutat_ibfk_1_BackReference
		/// </summary>
		[Association(ThisKey="ID", OtherKey="Mahlzeit", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Mahlzeitenthältzutat> Mahlzeitenthältzutatibfk { get; set; }

		/// <summary>
		/// mahlzeithatbilder_ibfk_1_BackReference
		/// </summary>
		[Association(ThisKey="ID", OtherKey="Mahlzeit", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Mahlzeithatbilder> Mahlzeithatbilderibfks { get; set; }

		/// <summary>
		/// preise_ibfk_1_BackReference
		/// </summary>
		[Association(ThisKey="ID", OtherKey="ID", CanBeNull=true, Relationship=Relationship.OneToOne, IsBackReference=true)]
		public Preise Preiseibfk { get; set; }

		#endregion
	}

	[Table("mahlzeitenthältzutat")]
	public partial class Mahlzeitenthältzutat
	{
		[Column, NotNull] public int Mahlzeit { get; set; } // int(11)
		[Column, NotNull] public int Zutat    { get; set; } // int(11)

		#region Associations

		/// <summary>
		/// mahlzeitenthältzutat_ibfk_1
		/// </summary>
		[Association(ThisKey="Mahlzeit", OtherKey="ID", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="mahlzeitenthältzutat_ibfk_1", BackReferenceName="Mahlzeitenthältzutatibfk")]
		public Mahlzeiten Ibfk { get; set; }

		/// <summary>
		/// mahlzeitenthältzutat_ibfk_2
		/// </summary>
		[Association(ThisKey="Zutat", OtherKey="ID", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="mahlzeitenthältzutat_ibfk_2", BackReferenceName="Mahlzeitenthältzutatibfk")]
		public Zutaten MahlzeitenthältzutatIbfk2 { get; set; }

		#endregion
	}

	[Table("mahlzeithatbilder")]
	public partial class Mahlzeithatbilder
	{
		[Column, NotNull] public int Mahlzeit { get; set; } // int(11)
		[Column, NotNull] public int Bild     { get; set; } // int(11)

		#region Associations

		/// <summary>
		/// mahlzeithatbilder_ibfk_1
		/// </summary>
		[Association(ThisKey="Mahlzeit", OtherKey="ID", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="mahlzeithatbilder_ibfk_1", BackReferenceName="Mahlzeithatbilderibfks")]
		public Mahlzeiten Ibfk { get; set; }

		/// <summary>
		/// mahlzeithatbilder_ibfk_2
		/// </summary>
		[Association(ThisKey="Bild", OtherKey="ID", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="mahlzeithatbilder_ibfk_2", BackReferenceName="Mahlzeithatbilderibfks")]
		public Bilder MahlzeithatbilderIbfk2 { get; set; }

		#endregion
	}

	[Table("mitarbeiter")]
	public partial class Mitarbeiter
	{
		[PrimaryKey, NotNull    ] public int    Nummer  { get; set; } // int(11)
		[Column,        Nullable] public string Telefon { get; set; } // varchar(15)
		[Column,        Nullable] public string Büro    { get; set; } // varchar(10)

		#region Associations

		/// <summary>
		/// mitarbeiter_ibfk_1
		/// </summary>
		[Association(ThisKey="Nummer", OtherKey="Nummer", CanBeNull=false, Relationship=Relationship.OneToOne, KeyName="mitarbeiter_ibfk_1", BackReferenceName="Mitarbeiteribfk")]
		public FhAngehörige Ibfk { get; set; }

		#endregion
	}

	[Table("preise")]
	public partial class Preise
	{
		[Column(),           PrimaryKey,  NotNull] public int      ID           { get; set; } // int(11)
		[Column(),                        NotNull] public int      Jahr         { get; set; } // int(11)
		[Column(),                        NotNull] public decimal  Gastpreis    { get; set; } // decimal(4,2)
		[Column("MA-Preis"),    Nullable         ] public decimal? MaPreis      { get; set; } // decimal(4,2)
		[Column(),              Nullable         ] public decimal? Studentpreis { get; set; } // decimal(4,2)

		#region Associations

		/// <summary>
		/// preise_ibfk_1
		/// </summary>
		[Association(ThisKey="ID", OtherKey="ID", CanBeNull=false, Relationship=Relationship.OneToOne, KeyName="preise_ibfk_1", BackReferenceName="Preiseibfk")]
		public Mahlzeiten Mahlzeiten { get; set; }

		#endregion
	}

	[Table("produkte", IsView=true)]
	public partial class Produkte
	{
		[Column(),              NotNull    ] public int    ID          { get; set; } // int(11)
		[Column(),              NotNull    ] public string Name        { get; set; } // varchar(50)
		[Column("verfügbar"),      Nullable] public bool?  Verfügbar   { get; set; } // tinyint(1)
		[Column("inKategorie"),    Nullable] public int?   InKategorie { get; set; } // int(11)
		[Column("vegetarisch"),    Nullable] public int?   Vegetarisch { get; set; } // int(1)
		[Column("vegan"),          Nullable] public int?   Vegan       { get; set; } // int(1)
		[Column("Alt-Text"),       Nullable] public string AltText     { get; set; } // varchar(255)
		[Column(),                 Nullable] public string Titel       { get; set; } // varchar(50)
		[Column(),                 Nullable] public byte[] Binärdaten  { get; set; } // mediumblob
	}

	[Table("studenten")]
	public partial class Studenten
	{
		[PrimaryKey, NotNull] public int    Nummer         { get; set; } // int(11)
		[Column,     NotNull] public int    Matrikelnummer { get; set; } // int(11)
		[Column,     NotNull] public string Studiengang    { get; set; } // enum('ET','INF','ISE','MCD','WI')

		#region Associations

		/// <summary>
		/// GeschriebenVOn_BackReference
		/// </summary>
		[Association(ThisKey="Nummer", OtherKey="Author", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Kommentare> GeschriebenVOns { get; set; }

		/// <summary>
		/// studenten_ibfk_1
		/// </summary>
		[Association(ThisKey="Nummer", OtherKey="Nummer", CanBeNull=false, Relationship=Relationship.OneToOne, KeyName="studenten_ibfk_1", BackReferenceName="Studentenibfk")]
		public FhAngehörige Ibfk { get; set; }

		#endregion
	}

	[Table("zutaten")]
	public partial class Zutaten
	{
		[PrimaryKey, NotNull] public int    ID          { get; set; } // int(5)
		[Column,     NotNull] public string Name        { get; set; } // varchar(50)
		[Column,     NotNull] public bool   Bio         { get; set; } // tinyint(1)
		[Column,     NotNull] public bool   Vegetarisch { get; set; } // tinyint(1)
		[Column,     NotNull] public bool   Vegan       { get; set; } // tinyint(1)
		[Column,     NotNull] public bool   Glutenfrei  { get; set; } // tinyint(1)

		#region Associations

		/// <summary>
		/// mahlzeitenthältzutat_ibfk_2_BackReference
		/// </summary>
		[Association(ThisKey="ID", OtherKey="Zutat", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Mahlzeitenthältzutat> Mahlzeitenthältzutatibfk { get; set; }

		#endregion
	}

	public static partial class DbwtDBStoredProcedures
	{
		#region Nutzerrolle

		public static int Nutzerrolle(this DataConnection dataConnection, int? ID, out string Role)
		{
			var ret = dataConnection.ExecuteProc("`Nutzerrolle`",
				new DataParameter("ID",   ID,   DataType.Int32),
				new DataParameter("Role", null, DataType.Char) { Direction = ParameterDirection.Output });

			Role = Converter.ChangeTypeTo<string>(((IDbDataParameter)dataConnection.Command.Parameters["Role"]).Value);

			return ret;
		}

		#endregion

		#region PreisFürNutzer

		public static IEnumerable<PreisFürNutzerResult> PreisFürNutzer(this DataConnection dataConnection, int? Nutzer, int? Mahlzeit)
		{
			return dataConnection.QueryProc<PreisFürNutzerResult>("`PreisFürNutzer`",
				new DataParameter("Nutzer",   Nutzer,   DataType.Int32),
				new DataParameter("Mahlzeit", Mahlzeit, DataType.Int32));
		}

		public partial class PreisFürNutzerResult
		{
			public decimal Preis { get; set; }
		}

		#endregion
	}

	public static partial class TableExtensions
	{
		public static Benutzer Find(this ITable<Benutzer> table, int Nummer)
		{
			return table.FirstOrDefault(t =>
				t.Nummer == Nummer);
		}

		public static Bestellungen Find(this ITable<Bestellungen> table, int Nummer)
		{
			return table.FirstOrDefault(t =>
				t.Nummer == Nummer);
		}

		public static Bilder Find(this ITable<Bilder> table, int ID)
		{
			return table.FirstOrDefault(t =>
				t.ID == ID);
		}

		public static Deklarationen Find(this ITable<Deklarationen> table, string Zeichen)
		{
			return table.FirstOrDefault(t =>
				t.Zeichen == Zeichen);
		}

		public static Fachbereiche Find(this ITable<Fachbereiche> table, int ID)
		{
			return table.FirstOrDefault(t =>
				t.ID == ID);
		}

		public static FhAngehörige Find(this ITable<FhAngehörige> table, int Nummer)
		{
			return table.FirstOrDefault(t =>
				t.Nummer == Nummer);
		}

		public static Gäste Find(this ITable<Gäste> table, int Nummer)
		{
			return table.FirstOrDefault(t =>
				t.Nummer == Nummer);
		}

		public static Kategorien Find(this ITable<Kategorien> table, int ID)
		{
			return table.FirstOrDefault(t =>
				t.ID == ID);
		}

		public static Kommentare Find(this ITable<Kommentare> table, int ID)
		{
			return table.FirstOrDefault(t =>
				t.ID == ID);
		}

		public static Mahlzeiten Find(this ITable<Mahlzeiten> table, int ID)
		{
			return table.FirstOrDefault(t =>
				t.ID == ID);
		}

		public static Mitarbeiter Find(this ITable<Mitarbeiter> table, int Nummer)
		{
			return table.FirstOrDefault(t =>
				t.Nummer == Nummer);
		}

		public static Preise Find(this ITable<Preise> table, int ID)
		{
			return table.FirstOrDefault(t =>
				t.ID == ID);
		}

		public static Studenten Find(this ITable<Studenten> table, int Nummer)
		{
			return table.FirstOrDefault(t =>
				t.Nummer == Nummer);
		}

		public static Zutaten Find(this ITable<Zutaten> table, int ID)
		{
			return table.FirstOrDefault(t =>
				t.ID == ID);
		}
	}
}
