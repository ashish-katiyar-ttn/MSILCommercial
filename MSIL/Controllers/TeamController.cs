using MSIL.Models;
using Sitecore.Data.Fields;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSIL.Controllers
{
    public class TeamController : Controller
    {
        // GET: Team
        public ActionResult Index()
        {
                var model = new Services();
                List<Service> slides = new List<Service>();

                var dataSource = RenderingContext.Current?.Rendering.Item;
                MultilistField slidesField = dataSource.Fields["Member"];


                if (slidesField?.Count > 0)
                {
                    var slideItems = slidesField.GetItems();

                    foreach (var slideItem in slideItems)
                    {
                        //Title
                        var titleField = slideItem.Fields["Title"];
                        var title = titleField?.Value;

                        //Sub Title
                        var subTitle = slideItem.Fields["Designation"];
                        var designation = subTitle.Value;

                        //Image
                        var image = new MvcHtmlString(FieldRenderer.Render
                            (slideItem, "Image", "class=img-fluid"));


                        slides.Add(new Service
                        {
                            Title = title,
                            SubTitle = designation,
                            Image = image,
                        });
                    }
                    model.Slides = slides;
                }
                return View(model);
        }
    }
}