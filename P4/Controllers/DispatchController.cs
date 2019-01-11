using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
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
				return Json(new HttpUnauthorizedResult(), JsonRequestBehavior.AllowGet);

			if (Request.Cookies["bestellung"] != null && Request.Cookies["bestellung"].Value.IsNullOrEmpty())
			{
				var dict = JsonConvert.DeserializeObject<Dictionary<int, int>>(Request.Cookies["bestellung"].Value);
				if (dict != null)
				{
					return Json(dict, JsonRequestBehavior.AllowGet);
				}

				return Json("Faulty Cookie Found", JsonRequestBehavior.AllowGet);
			}
			return Json("No Cookie Found", JsonRequestBehavior.AllowGet);
		}
	}
}