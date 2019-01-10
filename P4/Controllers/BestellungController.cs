using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DbModels;
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

	    public HttpCookie CreateCookie()
	    {
		    if (Request.Cookies["bestellungen"] == null)
		    {
			    var dict = new Dictionary<int, int> {{1, 1}, {2, 2}};
			    HttpCookie cookie = new HttpCookie("bestellung");
			    cookie.Value = JsonConvert.SerializeObject(dict);
				cookie.Expires = DateTime.Now.AddDays(1);
			    return cookie;
		    }

		    return null;
	    }

	    public ActionResult Set()
	    {
			Response.SetCookie(CreateCookie());
		    return View();
	    }

	    public PartialViewResult _Link()
	    {
		    int count = 0;
		    if (Request.Cookies["bestellung"] != null)
		    {
			    try
			    {
				    var dict = JsonConvert.DeserializeObject<Dictionary<int, int>>(
					    Request.Cookies["bestellung"].Value);
				    count = dict.Sum(e => e.Key);
			    }
			    catch (Exception)
			    {

			    }
			}
			return PartialView(new {count = count});
	    }
    }
}