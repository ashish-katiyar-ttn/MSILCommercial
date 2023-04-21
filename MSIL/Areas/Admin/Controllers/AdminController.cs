using MSIL.Models;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Mvc.Configuration;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSIL.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin/Admin
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            //Get Sitecore Item where you want to redirect

            return View(userModel);
        }
        [HttpPost]
        public ActionResult Login(UserModel um)
        {
            Item item = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse("{7200C77E-7657-4D5A-8E1E-5167DBEBD68D}"));

            var pathInfo = LinkManager.GetItemUrl(item, UrlOptions.DefaultOptions);

            return RedirectToRoute(MvcSettings.SitecoreRouteName, new { pathInfo = pathInfo.TrimStart(new char[] { '/' }) });

        }
        public ActionResult AboutManage()
        {
            // Get item from ID:
            Item item = Sitecore.Configuration.Factory.GetDatabase("master").GetItem("/sitecore/content/MSILCommercial/Data/About Content/about"); ;
            var model = new AboutViewModel()
            {
                Item = item
            };
            return View(model);
            //Get Sitecore Item where you want to redirect

        }
    }
}