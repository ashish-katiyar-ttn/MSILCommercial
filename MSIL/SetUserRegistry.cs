using Sitecore;
using Sitecore.Caching;
using Sitecore.Sites;
using System.Collections.Specialized;
using Sitecore.Security.Accounts;
using Sitecore.Diagnostics;
using Sitecore.Owin.Authentication.Pipelines.CookieAuthentication.SignedIn;
using System.Security.Claims;

namespace MSIL
{
	public class SetUserRegistry : SignedInProcessor
	{
		public override void Process(SignedInArgs args)
		{
			Assert.ArgumentNotNull(args, "args");
			User user = GetUser(args);
			// Make sure that the user has been authenicated else an error will occur when you save the profile
			if (user != null && user.IsAuthenticated)
			{
				SiteContext shellSite = SiteContext.GetSite("shell");
				string registryKey, registryValue, cacheKey;
				for (int i = 0; i < registryUpdates.Count; i++)
				{
					// Get the registry key and value
					registryKey = registryUpdates.GetKey(i);
					registryValue = registryUpdates.Get(i);
					// Clean up the registry key and replace Current_User with the loggedin user's name
					registryKey = StringUtil.Left(registryKey, 250);
					registryKey = registryKey.Replace("Current_User", user.Name.ToLowerInvariant());
					// Save the registry key into the User profile
					user.Profile[registryKey] = registryValue;
					user.Profile.Save();
					// Key to access the registry cache
					cacheKey = "registry_" + registryKey;
					// Registry cache is set per Site
					// At this point the Site is login so we need to switch the site to shell 
					// in order to update the correct site registry
					using (new SiteContextSwitcher(shellSite))
					{
						RegistryCache registryCache = CacheManager.GetRegistryCache(Context.Site);
						if (registryCache != null)
							registryCache.SetValue(cacheKey, registryValue);
					}
				}
			}
		}
		// This will return the user based on identity
		// Do not use Context.User because that will only come back as anonymous
		public User GetUser(SignedInArgs args)
		{
			User tempUser = null;
			// Get the user from the claims identity
			ClaimsIdentity identity = args.Context.Identity;
			string userName = (identity != null) ? identity.Name : null;
			if (!string.IsNullOrEmpty(userName))
			{
				// True will make sure that the user is authenticated
				tempUser = User.FromName(userName, true);
			}
			return tempUser;
		}
		// Here is where you would enter all of the user registry items you want changed when a user logs into the site
		NameValueCollection registryUpdates = new NameValueCollection()
		{
			{ "/Current_User/UserOptions.ContentEditor.ShowRawValues", "false" },
			{ "/Current_User/UserOptions.View.ShowHiddenItems", "true" },
			{ "/Current_User/UserOptions.View.ShowBucketItems", "true" },
			{ "/Current_User/UserOptions.ContentEditor.ShowQuickInfo", "true" },
			{ "/Current_User/UserOptions.View.ShowEntireTree", "true" },
			{ "/Current_User/Publish/SmartPublish", "true" },
			{ "/Current_User/Page Editor/Show/EditAllVersions", "on" },
			{ "/Current_User/SelectRendering/IsOpenPropertiesChecked", "true" },
			{ "/Current_User/Workbox/Page Size", "20" }
		};
	}
}