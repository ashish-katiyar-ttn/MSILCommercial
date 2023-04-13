using MSIL.Extensions;
using MSIL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using Sitecore.Data.Fields;

namespace MSIL.Controllers
{
    public class NavigationController : Controller
    {
        #region Build Navigation by Crawling the Tree
        public ActionResult Index()
        {
            var model = new NavigationViewModel();
            List<Navigation> navigations = new List<Navigation>();

            var homeItem = Sitecore.Context.Site.HomeItem();
            navigations.Add(BuildNavigation(homeItem));

            if (homeItem.HasChildren)
            {
                foreach (Item childItem in homeItem.Children)
                {
                        navigations.Add(BuildNavigation(childItem));
                }
            }
            model.Navigations = navigations;

            return View(model);
        }

        private Navigation BuildNavigation(Item item)
        {
            return new Navigation
            {
                NavigationTitle = item.Fields["Title"]?.Value,
                NavigationLink = item.Url(),
                ActiveClass = PageContext.Current.Item.ID == item.ID ? "active" : string.Empty
            };
        }
        #endregion
    }
}