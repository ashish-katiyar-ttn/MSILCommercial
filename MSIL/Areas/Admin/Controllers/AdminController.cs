using MSIL.Models;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Mvc.Configuration;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;
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
            var model = new AboutModel()
            {
                Title = item.Fields["Title"].Value,
				SubTitle = item.Fields["Sub_Title"].Value,
				Description = new MvcHtmlString(FieldRenderer.Render
						(item, "Description")).ToString(),
		};
            return View(model);
            //Get Sitecore Item where you want to redirect

        }
        [HttpPost]
		public ActionResult AboutManage(AboutModel about)
		{
			// Get item from ID:
			Item item = Sitecore.Configuration.Factory.GetDatabase("master").GetItem("/sitecore/content/MSILCommercial/Data/About Content/about"); ;
			item.Editing.BeginEdit();
			try
			{
				// Change the contents of the fields to update
				item.Fields["Title"].Value = about.Title;
				item.Fields["Sub_Title"].Value = about.SubTitle;
				item.Fields["Description"].Value = about.Description;
				// End edit writes the updates to the database:
				item.Editing.EndEdit();
			}
			catch (Exception ex)
			{
				// in case of an exception, you do not really
				// need to cancel editing, but it is good 
				// manners and it indicates that you know
				// what the code is doing
				item.Editing.CancelEdit();
			}
			using (new Sitecore.SecurityModel.SecurityDisabler())
			{
				string itemId = "05974ECC-4214-443D-BE6A-9FD354B94748";

				Database webdb = Sitecore.Configuration.Factory.GetDatabase("web");
				Database masterdb = Sitecore.Configuration.Factory.GetDatabase("master");

				ClearSitecoreDatabaseCache(masterdb);

				Item masterItem = masterdb.GetItem(new ID(itemId));

				// target databases
				Database[] databases = new Database[1] { webdb };

				Sitecore.Handle publishHandle = Sitecore.Publishing.PublishManager.PublishItem(masterItem, databases, webdb.Languages, true, false);

				ClearSitecoreDatabaseCache(webdb);
			}

			Item items = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse("{7200C77E-7657-4D5A-8E1E-5167DBEBD68D}"));

			var pathInfo = LinkManager.GetItemUrl(items, UrlOptions.DefaultOptions);

			return RedirectToRoute(MvcSettings.SitecoreRouteName, new { pathInfo = pathInfo.TrimStart(new char[] { '/' }) });

		}
		public void ClearSitecoreDatabaseCache(Database db)
		{
			// clear html cache
			Sitecore.Context.Site.Caches.HtmlCache.Clear();

			db.Caches.ItemCache.Clear();
			db.Caches.DataCache.Clear();

			//Clear prefetch cache
			foreach (var cache in Sitecore.Caching.CacheManager.GetAllCaches())
			{
				if (cache.Name.Contains(string.Format("Prefetch data({0})", db.Name)))
				{
					cache.Clear();
				}
			}
		}
	}
}