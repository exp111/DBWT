using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using P3.Models;

namespace P3.Controllers
{
    public class ZutatenController : Controller
    {
        // GET: Zutaten
        public ActionResult Index()
        {
	        string constr = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
	        Zutaten zutaten = new Zutaten()
	        {
				list = new List<Zutat>()
	        };
	        using (MySqlConnection con = new MySqlConnection(constr))
	        {
		        try
		        {
			        con.Open();
			        string query = "SELECT * FROM Zutaten ORDER BY Bio DESC, Name ASC";
			        using (MySqlCommand cmd = new MySqlCommand(query))
			        {
				        cmd.Connection = con;
				        using (MySqlDataReader reader = cmd.ExecuteReader())
				        {
					        while (reader.Read())
					        {
						        zutaten.list.Add(new Zutat()
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
			        }
			        con.Close();
		        }
		        catch (Exception e)
		        {
			        con.Close();
			        return View(zutaten);
		        }
	        }
			return View(zutaten);
        }
    }
}