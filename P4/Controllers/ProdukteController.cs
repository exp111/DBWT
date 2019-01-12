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
using LinqToDB.Common;
using MySql.Data.MySqlClient;
using P4.Models;

namespace P4.Controllers
{
	public class ProdukteController : Controller
	{
		// GET: Produkte
		public ActionResult Index()
		{
			string constr = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
			Produkte produkte = new Produkte()
			{
				rows = 5, columns = 4,
				kategorien = new List<Kategorie>(),
				mahlzeiten = new List<Mahlzeit>()

			};
			string filter = "";
			bool isPost = Request.HttpMethod == "POST";
			if (isPost)
			{
				bool checkCategory = !Request["filterCategory"].IsEmpty() && Request["filterCategory"] != "-1";
				string category = Request["filterCategory"];

				bool available = Request["filterAvailable"] == "available";

				bool vegetarian = Request["filterVegetarian"] == "vegetarian";

				bool vegan = Request["filterVegan"] == "vegan";
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
					ModelState.AddModelError("Error", e.Message);
					return View(produkte);
				}
			}

			return View(produkte);
		}

		[HttpGet]
		public ActionResult Detail(int id = -1)
		{
			if (id == -1)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			Mahlzeit mahlzeit = Mahlzeit.GetMahlzeit(id, Session, out string message);
			if (!message.IsNullOrEmpty())
				ModelState.AddModelError("Error", message);
			return View(mahlzeit);
		}

		[HttpPost]
		[ActionName("Detail")]
		public ActionResult DetailPost(int id)
		{
			if (String.IsNullOrEmpty(Session["user"] as string))
			{
				return RedirectToAction("Index", "Login");
			}
			String cookieName = $"bestellung{Session["user"]}";
			Response.SetCookie(BestellungController.AddToCookie(cookieName, Request.Cookies[cookieName], id, 1));
			ModelState.AddModelError("Affirmation", "In den Warenkorb gelegt.");
			return Detail(id);
		}
	}
}