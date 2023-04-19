using MSIL.Models;
using Newtonsoft.Json;
using Sitecore.Analytics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Sitecore.ContentSearch.Linq.Extensions.ReflectionExtensions;

namespace MSIL.Controllers
{
    public class FindADelearController : Controller
    {
        // GET: FindADelear
        public  ActionResult Index()
        {
            string url = "http://localhost:5094/api/Dealer/GetState";
            List<State> result = GetStates(url);

            return View(result);
        }
        public JsonResult GetCitylist(string stateId)
        {
            string url = "http://localhost:5094/api/Dealer/GetCity?stateId="+Convert.ToInt32(stateId);
            List<City> result = Getcity(url);
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDealers(string cityId)
        {
            string url = "http://localhost:5094/api/Dealer";
            List<Dealers> result = GetDealersList(url);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<State> GetStates(string path)
        {
            List<State> result = null;
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(path).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    result = JsonConvert.DeserializeObject<List<State>>(responseContent.ReadAsStringAsync().GetAwaiter().GetResult());
                }
            }
            return result;
        }
        
        public List<City> Getcity(string path)
        {
        
            List<City> result = null;
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(path).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    result = JsonConvert.DeserializeObject<List<City>>(responseContent.ReadAsStringAsync().GetAwaiter().GetResult());
                }
            }
            return result;
        }
        public List<Dealers> GetDealersList(string path)
        {

            List<Dealers> result = null;
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(path).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    result = JsonConvert.DeserializeObject<List<Dealers>>(responseContent.ReadAsStringAsync().GetAwaiter().GetResult());
                }
            }
            return result;
        }
    }
}