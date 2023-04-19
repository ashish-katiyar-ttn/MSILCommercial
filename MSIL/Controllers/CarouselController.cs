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
    public class CarouselController : Controller
    {
        public ActionResult Index()
        {
            var model = new CarouselModel();
            List<Slide> slides = new List<Slide>();

            var dataSource = RenderingContext.Current?.Rendering.Item;
            MultilistField slidesField = dataSource.Fields["Slides"];

    

            if (slidesField?.Count > 0)
            {
                var slideItems = slidesField.GetItems();

                foreach (var slideItem in slideItems)
                {
                    //Title
                    var titleField = slideItem.Fields["Title"];
                    var title = titleField?.Value;

                    //Sub Title
                    var subTitle = new MvcHtmlString(FieldRenderer.Render
                        (slideItem, "Sub_Title"));

                    //Image
                    var image = new MvcHtmlString(FieldRenderer.Render
                        (slideItem, "Image", "class=w-100"));

                    //Call to action
                    var callToAction = new MvcHtmlString(FieldRenderer.Render
                        (slideItem, "Call_To_Action"
                        , "class=btn btn-primary py-md-3 px-md-5 me-3 animated slideInLeft"));

                    slides.Add(new Slide
                    {
                        Title = title,
                        SubTitle = subTitle,
                        Image = image,
                        CallToAction = callToAction
                    });
                }
                model.Slides = slides;
            }
            return View(model);

            }

       
    }
}