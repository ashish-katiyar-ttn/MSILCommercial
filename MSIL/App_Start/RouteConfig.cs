using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MSIL
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            RouteTable.Routes.MapRoute("BookDetails", "BookDetails/GetBookDetails", new { controller = "BookDetails", action = "GetBookDetails" });
			RouteTable.Routes.MapRoute(
	"AllUserList",
	"Admin/AllUserList", new { controller = "Admin", action = "AllUserList" }
);
		}
    }
}
