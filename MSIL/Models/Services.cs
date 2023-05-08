using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSIL.Models
{
    public class Services
    {
        public string Search { get; set; }
        public List<Service> Slides { get; set; }
    }
    public class Service
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public MvcHtmlString Image { get; set; }
    }
}