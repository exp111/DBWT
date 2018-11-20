using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using P3.Models;

namespace P3.Controllers
{
    public class ProdukteController : Controller
    {
        // GET: Produkte
        public ActionResult Index()
        {
            return View();
        }

	    public ActionResult Details(int id)
	    {
		    if (id == null)
		    {
			    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
		    }

		    Mahlzeit m = new Mahlzeit()
		    {
			    ID = id
		    };
		    return View(m);
	    }
    }
}