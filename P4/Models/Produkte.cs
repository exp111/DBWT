using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using DbModels;
using LinqToDB.Data;
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
			msg = "";
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
				}
			}

			return mahlzeit;
		}
	}

	public class Produkte
	{
		public List<Mahlzeit> mahlzeiten { get; set; }
		public List<Kategorie> kategorien { get; set; }
		public int rows { get; set; }
		public int columns { get; set; }

		public static Produkte GetProdukte(HttpRequestBase request, out string msg)
		{
			Produkte produkte = new Produkte()
			{
				rows = 5,
				columns = 4,
				kategorien = new List<Kategorie>(),
				mahlzeiten = new List<Mahlzeit>()
			};
			msg = "";
			string filter = "";
			bool isPost = request.HttpMethod == "POST";
			if (isPost)
			{
				bool checkCategory = request["filterCategory"].Length > 0 && request["filterCategory"] != "-1";
				string category = request["filterCategory"];

				bool available = request["filterAvailable"] == "available";

				bool vegetarian = request["filterVegetarian"] == "vegetarian";

				bool vegan = request["filterVegan"] == "vegan";
				if (checkCategory || available || vegetarian || vegan)
				{
					filter = " WHERE";
					if (checkCategory)
						filter += $" inKategorie = {category} AND";
					if (available)
						filter += " verfügbar = 1 AND";
					if (vegetarian)
						filter += $" vegetarisch = 1 AND";
					if (vegan)
						filter += $" vegan = 1 AND";
					filter += " 1 = 1";
				}
			}

			string constr = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
			using (MySqlConnection con = new MySqlConnection(constr))
			{
				try
				{
					con.Open();
					// Get Kategorien
					string query = "SELECT ID, Bezeichnung, Parent FROM Kategorien";
					using (MySqlCommand cmd = new MySqlCommand(query, con))
					{
						using (MySqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								produkte.kategorien.Add(new Kategorie
								{
									ID = Convert.ToInt32(reader["ID"]),
									Bezeichnung = reader["Bezeichnung"].ToString(),
									Parent = (reader["Parent"] != DBNull.Value
										? Convert.ToInt32(reader["Parent"])
										: -1)
								});
							}
						}
					}

					// Get Mahlzeiten
					query = $"SELECT ID, Name, verfügbar, Titel, `Alt-Text`, Binärdaten FROM Produkte{filter}";
					using (MySqlCommand cmd = new MySqlCommand(query, con))
					{
						using (MySqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								produkte.mahlzeiten.Add(new Mahlzeit
								{
									ID = Convert.ToInt32(reader["ID"]),
									Name = reader["Name"].ToString(),
									Verfügbar = Convert.ToBoolean(reader["verfügbar"]),
									Bilder = (reader["Alt-Text"] != DBNull.Value
										? new List<Bild>()
										{
											new Bild()
											{
												Alttext = reader["Alt-Text"].ToString(),
												Titel = reader["Titel"].ToString(),
												Binärdaten =
													"data:image/jpg;base64," +
													Convert.ToBase64String((byte[]) reader["Binärdaten"])
											}
										}
										: null)
								});
							}
						}
					}
				}
				catch (Exception e)
				{
					con.Close();
					msg = e.Message;
				}
			}

			return produkte;
		}
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
	public class WarenkorbItem
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public double Preis { get; set; }
		public int Count { get; set; }

		public static List<WarenkorbItem> GetWarenkorb(Dictionary<int, int> oldDict, string userName, out string msg)
		{
			msg = "";
			List<WarenkorbItem> list = null;
			try
			{
				using (DbwtDB db = new DbwtDB())
				{
					int userId = 0;
					if (!String.IsNullOrEmpty(userName))
					{
						var result = db.Benutzer.Where(b => b.Nutzername.Equals(userName)).FirstOrDefault()
							?.Nummer;
						userId = result != null ? Convert.ToInt32(result) : 0;
					}

					list = db.Mahlzeiten.Where(m => oldDict.ContainsKey(m.ID))
						.Select(m => new WarenkorbItem
						{
							ID = m.ID,
							Name = m.Name,
							Count = oldDict[m.ID]
						})
						.ToList();

					list.ForEach(m =>
						m.Preis = db.QueryProc<double>("PreisFürNutzer",
								new { Nutzer = userId, Mahlzeit = m.ID })
							.FirstOrDefault());
				}
			}
			catch (Exception e)
			{
				msg = e.Message;
			}
			return list;
		}
	}
}

namespace LinqToDB.Data
{
	static class DataConnectionExtension
	{
		public static List<T> QueryProc<T>(this DataConnection db, string proc, object values)
		{
			List<DataParameter> parameters = new List<DataParameter>();
			foreach (var prop in values.GetType().GetProperties())
			{
				var name = prop.Name;
				var value = prop.GetValue(values);
				parameters.Add(new DataParameter(name, value));
			}
			var result = db.QueryProc<T>(proc, parameters.ToArray());
			return result.ToList();
		}
	}
}