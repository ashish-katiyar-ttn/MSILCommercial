using MSIL.Models;
using Sitecore;
using Sitecore.Common;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Mvc.Configuration;
using Sitecore.Mvc.Controllers;
using Sitecore.Pipelines.LoggedIn;
using Sitecore.Security.Accounts;
using Sitecore.Security.Authentication;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web.Authentication;
using Sitecore.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Web.Mvc;
using System.Web.Security;
using RoleList = MSIL.Models.RoleList;
using UserList = MSIL.Models.UserList;

namespace MSIL.Areas.Admin.Controllers
{
	public class AdminController : SitecoreController
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
			Sitecore.Security.Domains.Domain domain = Sitecore.Context.Domain;
			if (!string.IsNullOrEmpty(um.UserName) && !string.IsNullOrEmpty(um.Password))
			{
				string ticketID = TicketManager.GetCurrentTicketId();
				if (!string.IsNullOrEmpty(ticketID))
					TicketManager.RemoveTicket(ticketID);
					// Create virtual user
					Sitecore.Security.Accounts.User user =
						AuthenticationManager.BuildVirtualUser(string.Format(@"{0}\{1}", domain, um.UserName), false);
					if (user != null)
					{
					// Assign more roles or edit the user profile
					user.Profile.Name = string.Format(@"{0}\{1}", domain, um.UserName);
					user.Profile.Save();
					user.Profile.Reload();
					// Login the user
					AuthenticationManager.LoginVirtualUser(user);
					Item item = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse("{8226ED68-D939-4DB7-BBF3-D5F51475903B}"));
					bool isAccess = CheckReadAccess("{5C0CAE66-AE54-4C05-BE2D-1C6BE9F9B2B0}", Sitecore.Context.User.LocalName);
					Session["UserName"] = Sitecore.Context.User.Name;
					AuthenticationManager.SetActiveUser(Sitecore.Context.User.Name);
					var pathInfo = LinkManager.GetItemUrl(item, UrlOptions.DefaultOptions);
					return RedirectToRoute(MvcSettings.SitecoreRouteName, new { pathInfo = pathInfo.TrimStart(new char[] { '/' }) });
				}
			}
			return null;

		}
		public ActionResult CreateUsers()
		{
			CreateUser createUser = new CreateUser();
			if (IsAuthenticated())
			{
				List<RoleList> roles = new List<RoleList>();
				IEnumerable<Role> roleList = RolesInRolesManager.GetAllRoles();
				foreach (Role role in roleList)
				{
					roles.Add(RoleList(role));
				}
				createUser.RolesList = roles;
				return View(createUser);
			}
			else
				return null;
		}
		[HttpPost]
		public ActionResult CreateUsers(CreateUser createUser)
		{
			Sitecore.Security.Domains.Domain domain = Sitecore.Context.Domain;
			string userName = string.Format(@"{0}\{1}", domain, createUser.UserName);
			try
			{
				if (Membership.GetUser(userName)==null)
				{
					Membership.CreateUser(userName, createUser.Password, createUser.Email);
					// Edit the profile information
					Sitecore.Security.Accounts.User user = Sitecore.Security.Accounts.User.FromName(userName, true);
					Sitecore.Security.UserProfile userProfile = user.Profile;
					userProfile.FullName = userName;
					userProfile.Save();
					string parentRole = userName;
					string memberRole = createUser.Role;
					UserRoles.FromUser(Sitecore.Security.Accounts.User.FromName(userName, true)).Add(Role.FromName(memberRole));
					Item itm = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse("{5C0CAE66-AE54-4C05-BE2D-1C6BE9F9B2B0}"));
					var pathIfo = LinkManager.GetItemUrl(itm, UrlOptions.DefaultOptions);
					return RedirectToRoute(MvcSettings.SitecoreRouteName, new { pathInfo = pathIfo.TrimStart(new char[] { '/' }), username = Sitecore.Context.User.Name });
				}
			}
			catch (Exception ex)
			{
				Sitecore.Diagnostics.Log.Error(string.Format("Error in Client.Project.Security.UserMaintenance (AddUser): Message: {0}; Source:{1}", ex.Message, ex.Source), this);
			}
			Item item = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse("{8226ED68-D939-4DB7-BBF3-D5F51475903B}"));
			var pathInfo = LinkManager.GetItemUrl(item, UrlOptions.DefaultOptions);
			return RedirectToRoute(MvcSettings.SitecoreRouteName, new { pathInfo = pathInfo.TrimStart(new char[] { '/' }) });
		}
		public bool CheckReadAccess(string itemId, string UserName)
		{
			bool ReadAccess = false;

			if (Sitecore.Data.ID.IsID(itemId))
			{
				Item item = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse(itemId));
				if (item != null)
				{
					Sitecore.Security.Domains.Domain domain = Sitecore.Context.Domain;
					string domainUser = domain.Name + @"\" + UserName;
					//string domainUser = "sitecore" + @"\" + UserName;
					if (Sitecore.Security.Accounts.User.Exists(domainUser))
					{
						Sitecore.Security.Accounts.User user = Sitecore.Security.Accounts.User.FromName(domainUser, false);
						// UserSwitcher allows below code to run under a specific user 
						using (new Sitecore.Security.Accounts.UserSwitcher(user))
						{
							ReadAccess = item.Access.CanRead();
						}
					}
				}
			}
			return ReadAccess;
		}
		public ActionResult AboutManage()
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
		public ActionResult AllUserList(string username)
		{
			if (IsAuthenticated())
			{
				IFilterable<Sitecore.Security.Accounts.User> allUsers = UserManager.GetUsers();
				UserListModel userListModel = new UserListModel();
				userListModel.Username= username;
				List<UserList> userList = new List<UserList>();
				List<RoleLists> roleLists = new List<RoleLists>();
				foreach (Sitecore.Security.Accounts.User user in allUsers)
				{
					userList.Add(BuildUserList(user));
				}
				userListModel.userList = userList;
				IEnumerable<Role> roleList = RolesInRolesManager.GetAllRoles();
				foreach (Role role in roleList)
				{
					roleLists.Add(BuildRoleList(role));
				}
				userListModel.roleList = roleLists;
				return View(userListModel);
			}
			else
				return null;
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
		private UserList BuildUserList(Sitecore.Security.Accounts.User item)
		{
			return new UserList
			{
				AccountType = item.AccountType.ToString(),
				Description = item.Description,
				DisplayName = item.DisplayName,
				Domain=item.Domain==null?"": item.Domain.ToString(),
				Name=item.Name
			};
		}
		private RoleLists BuildRoleList(Role item)
		{
			return new RoleLists
			{
				AccountType = item.AccountType.ToString(),
				Domain=item.Domain.ToString(),
				Name=item.Name
			};
		}
		private RoleList RoleList(Role item)
		{
			return new RoleList
			{
				Id = item.Name,
				Name=item.Name
			};
		}
		public bool IsAuthenticated()
		{
			if (Sitecore.Context.User.IsAuthenticated)
				return true;
			else
			{
				Item item = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse(AccountsSettings.Fields.Error));
				var pathInfo = LinkManager.GetItemUrl(item, UrlOptions.DefaultOptions);
				Response.Redirect(pathInfo);
			}
			return false;
		}
		public static bool Login(string domainName, string userName, string password)
		{
			bool loginStatus=false;
			// created username in old way just to see if anything changed, didnt
			string username = domainName + @"\" + userName;
			string ticketID = TicketManager.GetCurrentTicketId();
			if (!string.IsNullOrEmpty(ticketID))
				TicketManager.RemoveTicket(ticketID);

			// created username in old way just to see if anything changed, didnt
			Sitecore.Security.Accounts.User virtualUser = Sitecore.Security.Authentication.AuthenticationManager.BuildVirtualUser(username, false);
			// manual population 
			virtualUser.Profile.FullName = username;
			virtualUser.Profile.Save();
			// login
			loginStatus = Sitecore.Security.Authentication.AuthenticationManager.LoginVirtualUser(virtualUser);
			// Sitecore.Security.Authentication.AuthenticationManager.Login(username, password, persistent: false, allowLoginToShell: false); 
			return loginStatus;
		}
		public static bool LoginAndSetUser(Sitecore.Security.Accounts.User user)
		{
			string ticketID = TicketManager.GetCurrentTicketId();
			if (!string.IsNullOrEmpty(ticketID))
				TicketManager.RemoveTicket(ticketID);
			return AuthenticationManager.Login(user);
		}
	}
}