using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
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
	        bool isPost = Request.HttpMethod == "POST";
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
			        query = "SELECT ID, Name, verfügbar, inKategorie FROM Mahlzeiten";
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
							        Kategorie = (reader["inKategorie"] != DBNull.Value
								        ? Convert.ToInt32(reader["inKategorie"])
								        : -1)
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
				    string query = $"SELECT Mahlzeiten.Name, Mahlzeiten.Beschreibung, Preise.Gastpreis, Preise.`MA-Preis` , Preise.Studentpreis FROM Mahlzeiten INNER JOIN Preise ON Mahlzeiten.ID = Preise.ID WHERE Mahlzeiten.ID =  {id}";
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
								    Preis = new Preis()
								    {
									    Gastpreis = Convert.ToDouble(reader["Gastpreis"]),
									    MAPreis = Convert.ToDouble(reader["MA-Preis"]),
									    Studentpreis = Convert.ToDouble(reader["Studentpreis"])
									},
									Zutaten = new List<string>(),
									Bilder = new List<Bild>()
							    };
						    }
					    }
				    }

				    if (mahlzeit != null)
				    {
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