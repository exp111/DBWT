using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using MySql.Data.MySqlClient;

namespace P2
{
	class ZutatenModel
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public bool Bio { get; set; }
		public bool Vegetarisch { get; set; }
		public bool Vegan { get; set; }
		public bool Glutenfrei { get; set; }
	}
	public class ZutatenController : Controller
	{
		// GET: Zutaten
		public ActionResult Index()
		{
			List<ZutatenModel> zutaten = new List<ZutatenModel>();
			string constr = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
			using (MySqlConnection con = new MySqlConnection(constr))
			{
				string query = "SELECT * FROM Zutaten";
				using (MySqlCommand cmd = new MySqlCommand(query))
				{
					cmd.Connection = con;
					con.Open();
					using (MySqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							zutaten.Add(new ZutatenModel
							{
								ID = Convert.ToInt32(reader["ID"]),
								Name = reader["Name"].ToString(),
								Bio = Convert.ToBoolean(reader["Bio"]),
								Vegetarisch = Convert.ToBoolean(reader["Vegetarisch"]),
								Vegan = Convert.ToBoolean(reader["Vegan"]),
								Glutenfrei = Convert.ToBoolean(reader["Glutenfrei"])
							});
						}
					}
					con.Close();
				}
			}

			return View(zutaten);
		}
	}
}