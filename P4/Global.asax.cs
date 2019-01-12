using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace P4
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

			// Init Linq2Db
			var constr = System.Configuration.ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
			LinqToDB.Data.DataConnection.DefaultSettings = new MySettings() { ConnectionString = constr };
		}
    }
}
