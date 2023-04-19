using MSIL.Models;
using Sitecore.Data.Fields;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSIL.Controllers
{
    public class MainController : SitecoreController
    {
        // GET: Main
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Services()
        {
            return View();
        }
        public ActionResult FindADealer()
        {
            return View();
        }

        // GET: Dealer
        [HttpPost]
        public JsonResult GetDealerDetails(string stateId)
        {
            string result = "success";
            //Book book = new Book();
            //if (Sitecore.Data.ID.IsID(itemId))
            //{
            //    Item item = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse(itemId));
            //    if (item != null)
            //    {
            //        book.BookTitle = item.Fields["Book Title"].Value;
            //        book.BookAuthor = item.Fields["Author"].Value;
            //        book.BookLanguage = item.Fields["Language"].Value;
            //    }
            //}
            return Json(result,JsonRequestBehavior.AllowGet);
        }
    }
}