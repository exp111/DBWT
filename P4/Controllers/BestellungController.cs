using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DbModels;
using LinqToDB.Common;
using LinqToDB.Data;
using LinqToDB.Tools;
using Newtonsoft.Json;
using P4.Models;

namespace P4.Controllers
{
    public class BestellungController : Controller
    {
        // GET: Bestellung
        public ActionResult Index()
        {
	        List<WarenkorbItem> list = new List<WarenkorbItem>();

	        if (Request.Cookies["bestellung"] != null)
	        {
		        var oldDict = JsonConvert.DeserializeObject<Dictionary<int, int>>(Request.Cookies["bestellung"].Value);
		        var constr = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
		        DataConnection.DefaultSettings = new MySettings() { ConnectionString = constr };
		        using (DbwtDB db = new DbwtDB())
		        {
			        int userId = 0;
			        if (!String.IsNullOrEmpty(Session["user"] as string))
			        {
						var result = db.Benutzer.Where(b => b.Nutzername.Equals(Session["user"])).FirstOrDefault()?.Nummer;
					    userId = result != null ? Convert.ToInt32(result) : 0;
			        }

					list = db.Mahlzeiten.Where(m => oldDict.ContainsKey(m.ID))
				        .Select(m => new WarenkorbItem
					        { ID = m.ID,
						        Name = m.Name,
						        Count = oldDict[m.ID],
						})
				        .ToList();

			        foreach (var mahlzeit in list)
			        {
				        mahlzeit.Preis =
					        db.QueryProc<double>("PreisFürNutzer", new {Nutzer = userId, Mahlzeit = mahlzeit.ID})
						        .FirstOrDefault();
			        }
				}
			}
	        
            return View(list);
        }

	    public static HttpCookie CreateCookie(string name, Dictionary<int, int> dict)
	    {
			HttpCookie cookie = new HttpCookie(name);
			cookie.Value = JsonConvert.SerializeObject(dict);
			cookie.Expires = DateTime.Now.AddDays(1);
			return cookie;
	    }

	    public static HttpCookie AddToCookie(HttpCookie cookie, int key, int value)
	    {
		    if (cookie != null)
		    {
			    try
			    {
				    var oldDict = JsonConvert.DeserializeObject<Dictionary<int, int>>(cookie.Value);
					if (oldDict.ContainsKey(key))
					{
						oldDict[key] += value;
					}
				    else
				    {
					    oldDict.Add(key, value);
				    }

				    return CreateCookie(cookie.Name, oldDict);
			    }
			    catch (Exception)
			    {
					// If we can't deserialize the cookie (faulty => create a new one)
			    }
			}

			return CreateCookie("bestellung", new Dictionary<int, int> { { key, value } });
	    }

	    public ActionResult Set()
	    {
			Response.SetCookie(CreateCookie("bestellung", new Dictionary<int, int>{{1, 1}}));
		    return View();
	    }

	    public ActionResult Update()
	    {
		    Response.SetCookie(AddToCookie(Request.Cookies["bestellung"], 1, 1));
		    return View("Set");
		}

	    public PartialViewResult _Link()
	    {
		    int count = 0;

			var cookie = Response.Cookies.AllKeys.Contains("bestellung")
			    ? Response.Cookies["bestellung"]
			    : Request.Cookies["bestellung"];

			if (cookie != null)
		    {
			    try
			    {
				    var dict = JsonConvert.DeserializeObject<Dictionary<int, int>>(
					    cookie.Value);
				    count = dict.Sum(e => e.Value);
			    }
			    catch (Exception)
			    {

			    }
			}
			return PartialView(new {count = count});
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