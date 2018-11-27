using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using MySql.Data.MySqlClient;
using P3.Models;

namespace P3.Controllers
{
    public class ProdukteController : Controller
    {
        // GET: Produkte
        public ActionResult Index()
        {
	        string constr = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
	        Produkte produkte = new Produkte()
	        {
		        rows = 2, columns = 4,
		        kategorien = new List<Kategorie>(),
		        mahlzeiten = new List<Mahlzeit>()

	        };
	        string filter = "";
	        bool isPost = Request.HttpMethod == "POST";
	        if (isPost)
	        {
		        bool checkCategory = !Request["filterCategory"].IsEmpty() && Request["filterCategory"] != "-1";
		        string category = Request["filterCategory"];

		        bool checkAvailable = Request["filterAvailable"] != null;
		        bool available = checkAvailable && Request["filterAvailable"] == "available";

		        bool checkVegetarian = Request["filterVegetarian"] != null;
		        bool vegetarian = checkVegetarian && Request["filterVegetarian"] == "vegetarian";

		        bool checkVegan = Request["filterVegan"] != null;
		        bool vegan = checkVegan && Request["filterVegan"] == "vegan";
		        if (checkCategory || checkAvailable || checkVegetarian || checkVegan)
		        {
			        filter = " WHERE";
			        if (checkCategory)
				        filter += $" inKategorie = {category} AND";
			        if (checkAvailable)
				        filter += $" verfügbar = {Convert.ToInt32(available)} AND";
					if (checkVegetarian)
						filter += $" vegetarisch = {Convert.ToInt32(vegetarian)} AND";
			        if (checkVegan)
				        filter += $" vegan = {Convert.ToInt32(vegan)} AND";
					filter += " 1 = 1";
		        }
	        }
	        using (MySqlConnection con = new MySqlConnection(constr))
	        {
		        try
		        {
			        con.Open();
			        // Get Kategorien
			        string query = "SELECT ID, Bezeichnung, Parent FROM Kategorien";
			        using (MySqlCommand cmd = new MySqlCommand(query))
			        {
				        cmd.Connection = con;
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
			        query = $"SELECT Mahlzeiten.ID, Mahlzeiten.Name, Mahlzeiten.verfügbar, Bilder.Titel, Bilder.`Alt-Text`, Bilder.Binärdaten FROM (SELECT DISTINCT Mahlzeiten.ID, Mahlzeiten.Name, Mahlzeiten.verfügbar FROM (Mahlzeiten LEFT JOIN MahlzeitEnthältZutat ON Mahlzeit = ID) LEFT JOIN Zutaten ON zutaten.ID = Zutat{filter} LIMIT 8) AS Mahlzeiten LEFT JOIN MahlzeitHatBilder ON mahlzeithatbilder.Mahlzeit = Mahlzeiten.ID LEFT JOIN Bilder ON mahlzeithatbilder.Bild = Bilder.ID GROUP BY Mahlzeiten.ID";
			        using (MySqlCommand cmd = new MySqlCommand(query))
			        {
				        cmd.Connection = con;
				        using (MySqlDataReader reader = cmd.ExecuteReader())
				        {
					        while (reader.Read())
					        {
						        produkte.mahlzeiten.Add(new Mahlzeit
						        {
									ID = Convert.ToInt32(reader["ID"]),
							        Name = reader["Name"].ToString(),
							        Verfügbar = Convert.ToBoolean(reader["verfügbar"]),
									Bilder = (reader["Alt-Text"] != DBNull.Value ? new List<Bild>()
									{
										new Bild()
											{
										Alttext = reader["Alt-Text"].ToString(),
										Titel = reader["Titel"].ToString(),
										Binärdaten = "data:image/jpg;base64," + Convert.ToBase64String((byte[])reader["Binärdaten"])
												}
									} : null)
								});
					        }
				        }
			        }
		        }
		        catch (Exception e)
		        {
			        con.Close();
			        ModelState.AddModelError("Error", e.Message);
			        return View(produkte);
		        }
	        }

	        return View(produkte);
        }

	    public ActionResult Detail(int id = -1)
	    {
		    if (id == -1)
		    {
			    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
		    }

		    string constr = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
		    Mahlzeit mahlzeit = null;
		    List<Bild> bilder = new List<Bild>();
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
					    if (Session["user"] != null)
					    {
							//FIXME: unsafe af
						    query = $"SELECT Nummer from Benutzer WHERE Nutzername = \"{Session["user"]}\"";
						    using (MySqlCommand cmd = new MySqlCommand(query))
						    {
							    cmd.Connection = con;
							    var result = cmd.ExecuteScalar();
								userId = result != null ? Convert.ToInt32(result) : 0;
						    }
					    }

					    // Preis
						query = $"CALL PreisFürNutzer({userId}, {mahlzeit.ID})";
					    using (MySqlCommand cmd = new MySqlCommand(query))
					    {
						    cmd.Connection = con;
						    mahlzeit.Preis = Convert.ToDouble(cmd.ExecuteScalar().ToString());
					    }

						// Zutaten
						query = $"SELECT Zutaten.Name FROM (SELECT Zutat FROM MahlzeitEnthältZutat WHERE Mahlzeit = {id}) AS AZutaten INNER JOIN Zutaten ON AZutaten.Zutat = Zutaten.ID";
					    using (MySqlCommand cmd = new MySqlCommand(query))
					    {
						    cmd.Connection = con;
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
					    using (MySqlCommand cmd = new MySqlCommand(query))
					    {
						    cmd.Connection = con;
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
				    ModelState.AddModelError("Error", e.Message);
					return View(mahlzeit);
			    }
		    }
		    return View(mahlzeit);
		}
    }
}