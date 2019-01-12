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

			List<Bestellungen> list = DbModels.Bestellungen.GetDispatch(1, out string msg);
			if (!msg.IsNullOrEmpty())
				return Json(msg, JsonRequestBehavior.AllowGet);

			return Json(list, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Index()
		{
			return View();
		}
	}
}