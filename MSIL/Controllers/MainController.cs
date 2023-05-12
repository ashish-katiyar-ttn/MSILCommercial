using MSIL.Models;
using Nest;
using Newtonsoft.Json;
using Sitecore.Analytics;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Mvc.Configuration;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Pipelines;
using Sitecore.Pipelines.LoggedIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using Sitecore.Diagnostics;
using Sitecore.Security.Accounts;
using Sitecore.Security.Authentication;
using Sitecore.Pipelines;
using MSIL.Pipelines;
using MSIL.Repositories;
using static Sitecore.Configuration.Settings;
using Sitecore.Data;
using Sitecore.Web.UI.WebControls;

namespace MSIL.Controllers
{
	public class MainController : SitecoreController
    {
        // GET: Main
        public ActionResult Index()
        {
            return View();
        }
		public ActionResult AdminLogin()
		{
			UserModel userModel = new UserModel();
			//Get Sitecore Item where you want to redirect

			return View(userModel);
		}
		[HttpPost]
		public ActionResult Login(UserModel um)
		{
			var user = Login(um.UserName, um.Password);
			if (user == null)
			{
				ModelState.AddModelError("invalidCredentials",  "Username or password is not valid.");
			}
			else
			{
				RedirectToActionURL(AccountsSettings.Fields.AfterLoginPage);
			}

			return View(um);
		}
		public ActionResult Services()
        {
            return View();
        }
		public ActionResult Service()
		{
			return View();
		}
		public ActionResult FindADealer()
        {
            return View();
        }
		public ActionResult CreateUser()
		{
			return View();
		}
		[HttpPost]
		public ActionResult CreateUserRedirect(UserListModel userListModel)
		{
			Item item = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse("{8226ED68-D939-4DB7-BBF3-D5F51475903B}"));
			var pathInfo = LinkManager.GetItemUrl(item, UrlOptions.DefaultOptions);
			return RedirectToRoute(MvcSettings.SitecoreRouteName, new { pathInfo = pathInfo.TrimStart(new char[] { '/' }), username = userListModel.Username });
		}
		public virtual ActionResult RedirectToActionURL(ID itemId)
		{
			if (Sitecore.Context.User.IsAuthenticated &&(Sitecore.Context.User.LocalName.ToLower() != "Anonymous".ToLower()))
			{
				Item item = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse(itemId));
				var pathInfo = LinkManager.GetItemUrl(item, UrlOptions.DefaultOptions);
				Response.Redirect(pathInfo);
			}
			else
			{
				Item item = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse(AccountsSettings.Fields.Error));
				var pathInfo = LinkManager.GetItemUrl(item, UrlOptions.DefaultOptions);
				Response.Redirect(pathInfo);
			}
			return null;
		}
		[HttpPost]
		public ActionResult Logout()
		{
			if (Sitecore.Context.User.IsAuthenticated)
			{
				LogoutUser();
				Item item = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse(AccountsSettings.Fields.LoginPage));
				var pathInfo = LinkManager.GetItemUrl(item, UrlOptions.DefaultOptions);
				Response.Redirect(pathInfo);
			}
			else
			{
				Item item = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse(AccountsSettings.Fields.Error));
				var pathInfo = LinkManager.GetItemUrl(item, UrlOptions.DefaultOptions);
				Response.Redirect(pathInfo);
			}
			return null;
		}
		// GET: Dealer
		[HttpPost]
        public JsonResult GetDealerDetails(string stateId)
        {
            string result = "success";
            return Json(result,JsonRequestBehavior.AllowGet);
        }
		public JsonResult GetCar(string searchtxt)
		{
			string url = "http://localhost:5094/api/Car";
			Car[] apiresult = GetCarList(url);
			Car[] result=apiresult.Where(a=>a.Name.Contains(searchtxt)).ToArray();
			// Connecting to Elasticsearch
			var node = new Uri("http://localhost:9200/");
			var settings = new ConnectionSettings(node);
			var client = new ElasticClient(settings);
			string indexName = "car-index";
			var response = client.Indices.Create(indexName,
					index => index.Map<Car>(
						x => x.AutoMap()
					));

			var indexResponse = client.IndexMany(result, indexName);
			var getResponse = client.Get<Car>(1, i => i.Index(indexName));
			Car articleDocument = getResponse.Source;

			return Json(result, JsonRequestBehavior.AllowGet);
		}
		public Car[] GetCarList(string path)
		{

			Car[] result = null;
			using (var client = new HttpClient())
			{
				var response = client.GetAsync(path).GetAwaiter().GetResult();
				if (response.IsSuccessStatusCode)
				{
					var responseContent = response.Content;
					result = JsonConvert.DeserializeObject<Car[]>(responseContent.ReadAsStringAsync().GetAwaiter().GetResult());
				}
			}
			return result;
		}
		public ActionResult Dashboard()
		{
			return View();
		}
		public ActionResult DashboardData()
		{
			if (IsAuthenticated())
				return View();
			else
				return null;
		}
		public bool IsAuthenticated()
		{
			if(Sitecore.Context.User.IsAuthenticated)
				return true;
			else
			{
				Item item = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse(AccountsSettings.Fields.Error));
				var pathInfo = LinkManager.GetItemUrl(item, UrlOptions.DefaultOptions);
				Response.Redirect(pathInfo);
			}
			return false;
		}
		public User Login(string userName, string password)
		{
			var accountName = string.Empty;
			var domain = Sitecore.Context.Domain;
			if (domain != null)
			{
				accountName = domain.GetFullName(userName);
			}

			var result = AuthenticationManager.Login(accountName, password);
			if (!result)
			{
				return null;
			}

			var user = AuthenticationManager.GetActiveUser();
			RunLoggedIn(user);
			return user;
		}
		public bool RunLoggedIn(User user)
		{
			var args = new LoggedInPipelineArgs()
			{
				User = user,
				Source = user.GetDomainName(),
				UserName = user.LocalName,
				ContactId = Tracker.Current?.Contact?.ContactId
			};
			CorePipeline.Run("accounts.loggedIn", args);
			return args.Aborted;
		}
		public void LogoutUser()
		{
			var user = AuthenticationManager.GetActiveUser();
			AuthenticationManager.Logout();
			if (user != null)
			{
				RunLoggedOut(user);
			}
		}
		public bool RunLoggedOut(User user)
		{
			var args = new AccountsPipelineArgs()
			{
				User = user,
				UserName = user.Name
			};
			CorePipeline.Run("accounts.loggedOut", args);
			return args.Aborted;
		}
		public ActionResult Publish()
		{
			return View();
		}
		public ActionResult Publisher()
		{
			if (IsAuthenticated())
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
			}
			else
				return null;
		}
		[HttpPost]
		public ActionResult Publisher(AboutModel about)
		{
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

			Item items = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse(AccountsSettings.Fields.AfterLoginPage));

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