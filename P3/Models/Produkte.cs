using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace P3.Models
{
	public class Kategorie
	{
		public int ID { get; set; }
		public String Bezeichnung { get; set; }
		public int HatBilder { get; set; }
		public int Parent { get; set; }
	}

	public class Preis
	{
		public int ID { get; set; }
		public double Gastpreis { get; set; }
		public double MAPreis { get; set; }
		public double Studentpreis { get; set; }
	}

	class Bild
	{
		public int ID { get; set; }
		public string Titel { get; set; }
		public string Alttext { get; set; }
		public string binärdaten { get; set; }
	}

	public class Mahlzeit
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Beschreibung { get; set; }
		public int Vorrat { get; set; }
		public bool Verfügbar { get; set; }
		public List<Kategorie> Kategorien { get; set; }
		public Preis Preis { get; set; }

		public List<String> Zutaten { get; set; }
	}

	public class Produkte
	{
		public List<Mahlzeit> mahlzeiten { get; set; }
	}
}