using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using LinqToDB.Common;
using MySql.Data.MySqlClient;
using P4.Models;

namespace P4.Controllers
{
	public class ProdukteController : Controller
	{
		// GET: Produkte
		public ActionResult Index()
		{
			Produkte produkte = Produkte.GetProdukte(Request, out string message);
			if (!message.IsNullOrEmpty())
				ModelState.AddModelError("Error", message);

			return View(produkte);
		}

		[HttpGet]
		public ActionResult Detail(int id = -1)
		{
			if (id == -1)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			Mahlzeit mahlzeit = Mahlzeit.GetMahlzeit(id, Session, out string message);
			if (!message.IsNullOrEmpty())
				ModelState.AddModelError("Error", message);
			return View(mahlzeit);
		}

		[HttpPost]
		[ActionName("Detail")]
		public ActionResult DetailPost(int id)
		{
			if (String.IsNullOrEmpty(Session["user"] as string))
			{
				return RedirectToAction("Index", "Login");
			}
			String cookieName = $"bestellung{Session["user"]}";
			Response.SetCookie(BestellungController.AddToCookie(cookieName, Request.Cookies[cookieName], id, 1));
			ModelState.AddModelError("Affirmation", "In den Warenkorb gelegt.");
			return Detail(id);
		}
	}
}