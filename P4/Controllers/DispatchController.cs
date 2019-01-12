using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DbModels;
using LinqToDB.Common;
using Newtonsoft.Json;

namespace P4.Controllers
{
    public class DispatchController : Controller
    {
		// GET: Dispatch
		public JsonResult Bestellungen()
		{
			var xAuthorize = Request.Headers.GetValues("X-Authorize");
			if (xAuthorize.IsNullOrEmpty() || !xAuthorize.First().Equals("ohShitHereComesDatBoi"))
				return Json(new HttpUnauthorizedResult("Sie sind nicht authorisiert"), JsonRequestBehavior.AllowGet);

			using (DbwtDB db = new DbwtDB())
			{
				try
				{
					List<Bestellungen> list = db.Bestellungen.Where(b => DateTime.Now < b.Abholzeitpunkt //after now
																	&& DateTime.Now.AddHours(1) > b.Abholzeitpunkt) //in max 1h
																	.ToList();
					return Json(list, JsonRequestBehavior.AllowGet);
				}
				catch (Exception e)
				{
					return Json(e.Message, JsonRequestBehavior.AllowGet);
				}
			}
		}

		public ActionResult Index()
		{
			return View();
		}
	}
}