using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using MySql.Data.MySqlClient;

namespace P4.Models
{
	public class Kategorie
	{
		public int ID { get; set; }
		public String Bezeichnung { get; set; }
		public int HatBilder { get; set; }
		public int Parent { get; set; }
	}

	public class Bild
	{
		public int ID { get; set; }
		public string Titel { get; set; }
		public string Alttext { get; set; }
		public string Binärdaten { get; set; }
	}

	public class Mahlzeit
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Beschreibung { get; set; }
		public int Vorrat { get; set; }
		public bool Verfügbar { get; set; }
		public int Kategorie { get; set; }
		public double Preis { get; set; }
		public List<string> Zutaten { get; set; }
		public List<Bild> Bilder { get; set; }

		public static Mahlzeit GetMahlzeit(int id, HttpSessionStateBase session, out string msg)
		{
			string constr = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
			Mahlzeit mahlzeit = null;
			using (MySqlConnection con = new MySqlConnection(constr))
			{
				try
				{
					con.Open();
					// Get Mahlzeit Details
					string query = $"SELECT Mahlzeiten.Name, Mahlzeiten.Beschreibung FROM Mahlzeiten WHERE Mahlzeiten.ID = {id}";
					using (MySqlCommand cmd = new MySqlCommand(query))
					{
						cmd.Connection = con;
						using (MySqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								mahlzeit = new Mahlzeit()
								{
									ID = id,
									Name = reader["Name"].ToString(),
									Beschreibung = reader["Beschreibung"].ToString(),
									Zutaten = new List<string>(),
									Bilder = new List<Bild>()
								};
							}
						}
					}

					if (mahlzeit != null)
					{
						// User //TODO: maybe move into a procedure? (PreisFürNutzer(Name, id)
						int userId = 0;
						if (!String.IsNullOrEmpty(session["user"] as string))
						{
							query = $"SELECT Nummer from Benutzer WHERE Nutzername = @name";
							using (MySqlCommand cmd = new MySqlCommand(query, con))
							{
								cmd.Parameters.AddWithValue("name", session["user"]);
								var result = cmd.ExecuteScalar();
								userId = result != null ? Convert.ToInt32(result) : 0;
							}
						}

						// Preis
						query = $"CALL PreisFürNutzer({userId}, {mahlzeit.ID})";
						using (MySqlCommand cmd = new MySqlCommand(query, con))
						{
							mahlzeit.Preis = Convert.ToDouble(cmd.ExecuteScalar().ToString());
						}

						// Zutaten
						query = $"SELECT Zutaten.Name FROM (SELECT Zutat FROM MahlzeitEnthältZutat WHERE Mahlzeit = {id}) AS AZutaten INNER JOIN Zutaten ON AZutaten.Zutat = Zutaten.ID";
						using (MySqlCommand cmd = new MySqlCommand(query, con))
						{
							using (MySqlDataReader reader = cmd.ExecuteReader())
							{
								while (reader.Read())
								{
									mahlzeit.Zutaten.Add(reader["Name"].ToString());
								}
							}
						}

						// Bilder
						query = $"SELECT Bilder.`Alt-Text`, Bilder.Titel, Bilder.Binärdaten FROM (SELECT Bild FROM MahlzeitHatBilder WHERE Mahlzeit = {id}) AS ABilder INNER JOIN Bilder ON ABilder.Bild = Bilder.ID";
						using (MySqlCommand cmd = new MySqlCommand(query, con))
						{
							using (MySqlDataReader reader = cmd.ExecuteReader())
							{
								while (reader.Read())
								{
									mahlzeit.Bilder.Add(new Bild()
									{
										Alttext = reader["Alt-Text"].ToString(),
										Titel = reader["Titel"].ToString(),
										Binärdaten = "data:image/jpg;base64," + Convert.ToBase64String((byte[])reader["Binärdaten"])
									});
								}
							}
						}
					}

					con.Close();
				}
				catch (Exception e)
				{
					con.Close();
					msg = e.Message;
					return mahlzeit;
				}
			}

			msg = "";
			return mahlzeit;
		}
	}

	public class Produkte
	{
		public List<Mahlzeit> mahlzeiten { get; set; }
		public List<Kategorie> kategorien { get; set; }
		public int rows { get; set; }
		public int columns { get; set; }
	}

	public class Zutat
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public bool Bio { get; set; }
		public bool Vegetarisch { get; set; }
		public bool Vegan { get; set; }
		public bool Glutenfrei { get; set; }
	}
	public class Zutaten
	{
		public List<Zutat> list;
	}
}