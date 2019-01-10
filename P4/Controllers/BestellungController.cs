using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DbModels;
using LinqToDB.Common;
using LinqToDB.Tools;
using Newtonsoft.Json;

namespace P4.Controllers
{
    public class BestellungController : Controller
    {
        // GET: Bestellung
        public ActionResult Index()
        {
	        List<Benutzer> list;
			var constr = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
			using (DbwtDB db = new DbwtDB(constr))
			{
				list = db.Benutzer.Where(b => b.Aktiv).ToList();
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