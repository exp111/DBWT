using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using MySql.Data.MySqlClient;
using P3.Models;

namespace P3.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
			Login login = new Login(){LoggedIn = false};

			// Check if we're already logged in
	        if (!String.IsNullOrEmpty(Session["user"] as string) && !String.IsNullOrEmpty(Session["role"] as string))
	        {
		        login.LoggedIn = true;
		        login.Username = Session["user"].ToString();
		        login.Role = Session["role"].ToString();

				return View(login);
	        }

	        bool isPost = Request.HttpMethod == "POST";
			if (isPost)
			{
				string constr = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
				using (MySqlConnection con = new MySqlConnection(constr))
		        {
			        try
			        {
				        con.Open();
						//FIXME: unsafe af; use parameters instead
				        string query = $"SELECT Nummer, Salt, Hash FROM Benutzer WHERE Nutzername = \"{Request["loginName"]}\" LIMIT 1";
				        using (MySqlCommand cmd = new MySqlCommand(query))
				        {
					        cmd.Connection = con;
					        using (MySqlDataReader reader = cmd.ExecuteReader())
					        {
						        if (reader.Read())
						        {
							        login.LoggedIn = true;
							        login.Username = Request["loginName"];
							        login.ID = Convert.ToInt32(reader["Nummer"]);
							        login.Salt = reader["Salt"].ToString();
							        login.Hash = reader["Hash"].ToString();
						        }
					        }
				        }

				        if (login.LoggedIn)
				        {
					        query = $"CALL NutzerrollePrint({login.ID})";
					        using (MySqlCommand cmd = new MySqlCommand(query))
					        {
						        cmd.Connection = con;
						        login.Role = cmd.ExecuteScalar().ToString();
					        }
				        }
				        con.Close();
			        }
			        catch (Exception e)
			        {
				        con.Close();
				        ModelState.AddModelError("Error", e.Message);
				        login.Failed = true;
				        return View(login);
			        }
		        }

				if (!login.LoggedIn) //user not found/pw wrong
				{
					ModelState.AddModelError("Error", "Das hat nicht geklappt! Bitte versuchen Sie es erneut.");
					login.Failed = true;
					return View(login);
				}

				//P3.PasswordSecurity.PasswordStorage.CreateHash(Request["loginPassword"]);
		        Session["user"] = login.Username;
		        Session["role"] = login.Role;
			}
            return View(login);
        }

	    public ActionResult Logout()
	    {
		    if (!String.IsNullOrEmpty(Session["user"] as string) || !String.IsNullOrEmpty(Session["role"] as string))
		    {
			    Session["user"] = null;
			    Session["role"] = null;
				ModelState.AddModelError("Success", "Erfolgreich abgemeldet.");
		    }
			else
				ModelState.AddModelError("Error", "Da ist etwas schief gegangen!");

			return View();
	    }
    }
}
