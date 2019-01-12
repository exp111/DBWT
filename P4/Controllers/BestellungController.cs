using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DbModels;
using LinqToDB;
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

			if (!String.IsNullOrEmpty(Session["user"] as string))
			{
				string cookieName = $"bestellung{Session["user"]}";
				var cookie = Response.Cookies.AllKeys.Contains(cookieName)
					? Response.Cookies[cookieName]
					: Request.Cookies[cookieName];

				if (cookie != null)
				{
					var oldDict = JsonConvert.DeserializeObject<Dictionary<int, int>>(cookie.Value);
					if (oldDict != null)
					{
						list = WarenkorbItem.GetWarenkorb(oldDict, Session["user"] as string, out string message);
						if (!message.IsNullOrEmpty())
							ModelState.AddModelError("Error", message);
					}
				}
			}

			return View(list);
		}

		[HttpPost]
		[ActionName("Index")]
		public ActionResult IndexPost()
		{
			if (!Request.Form.IsNullOrEmpty())
			{
				if (String.IsNullOrEmpty(Session["user"] as string))
					return RedirectToAction("Index", "Login");

				string cookieName = $"bestellung{Session["user"]}";

				if (Request.Form.AllKeys.Contains("delete"))
				{
					Response.Cookies[cookieName].Value = "";
				}
				else if (Request.Form.AllKeys.Contains("update"))
				{
					var oldDict = JsonConvert.DeserializeObject<Dictionary<int, int>>(Request.Cookies[cookieName].Value);
					if (oldDict != null)
					{
						foreach (string input in Request.Form)
						{
							if (!Int32.TryParse(input, out int intput))
								continue;

							if (oldDict.ContainsKey(intput))
							{
								if (!Int32.TryParse(Request.Form[input], out int intputValue))
									continue;
								if (intputValue > 0)
								{
									oldDict[intput] = intputValue;
								}
								else
								{
									oldDict.Remove(intput);
								}
							}
						}

						Response.SetCookie(CreateCookie(cookieName, oldDict));
					}
				}
				else if (Request.Form.AllKeys.Contains("order"))
				{
					DbModels.Bestellungen.Order(Request.Form, Session["user"] as string, out string message);
					Response.Cookies[cookieName].Value = "";
					if (!message.IsNullOrEmpty())
						ModelState.AddModelError("Error", message);
					ModelState.AddModelError("Affirmation", "Erfolgreich bestellt!");
				}
			}

			return Index();
		}

		public static HttpCookie CreateCookie(string name, Dictionary<int, int> dict)
		{
			HttpCookie cookie = new HttpCookie(name);
			cookie.Value = JsonConvert.SerializeObject(dict);
			cookie.Expires = DateTime.Now.AddDays(1);
			return cookie;
		}

		public static HttpCookie AddToCookie(string name, HttpCookie cookie, int key, int value)
		{
			if (cookie != null)
			{
				try
				{
					var oldDict = JsonConvert.DeserializeObject<Dictionary<int, int>>(cookie.Value);
					if (oldDict != null)
					{
						if (oldDict.ContainsKey(key))
						{
							oldDict[key] += value;
						}
						else
						{
							oldDict.Add(key, value);
						}

						return CreateCookie(name, oldDict);
					}
				}
				catch (Exception)
				{
					// If we can't deserialize the cookie (faulty => create a new one)
				}
			}

			return CreateCookie(name, new Dictionary<int, int> { { key, value } });
		}

		public PartialViewResult _Link()
		{
			int count = 0;

			if (!String.IsNullOrEmpty(Session["user"] as string))
			{
				string cookieName = $"bestellung{Session["user"]}";
				var cookie = Response.Cookies.AllKeys.Contains(cookieName)
					? Response.Cookies[cookieName]
					: Request.Cookies[cookieName];

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
			}
			return PartialView(new { count = count });
		}
	}
}