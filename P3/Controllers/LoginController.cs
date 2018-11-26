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
	        }
            return View(login);
        }
    }
}