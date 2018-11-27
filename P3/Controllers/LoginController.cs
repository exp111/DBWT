using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
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
		        login.Type = Session["role"].ToString();

				return View(login);
	        }

	        bool isPost = Request.HttpMethod == "POST";
			if (isPost)
	        {
		        if (Request["loginName"] == "a" || Request["loginName"].IsEmpty() || Request["loginPassword"].IsEmpty())
		        {
			        ModelState.AddModelError("Error", "Das hat nicht geklappt! Bitte versuchen Sie es erneut.");
			        login.Failed = true;
			        return View(login);
		        }
		        login.LoggedIn = true;
		        login.Username = Request["loginName"].ToString();
		        login.Type = "Student";
		        Session["user"] = login.Username;
		        Session["role"] = login.Type;
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