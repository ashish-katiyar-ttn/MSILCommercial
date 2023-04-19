using MSIL.Models;
using Sitecore.Mvc.Presentation;
using System.Web.Mvc;

namespace MSIL.Controllers
{
    public class AboutController : Controller
    {
        // GET: About
        public ActionResult Index()
        {
            var model = new AboutViewModel()
            {
                Item = RenderingContext.Current?.Rendering.Item
            };
            return View(model);
        }
    }
}