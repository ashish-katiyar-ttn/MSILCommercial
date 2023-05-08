using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Sitecore.Data;
namespace MSIL
{
	public struct AccountsSettings
	{
		public static readonly ID ID = new ID("{4C7D90D2-CEAA-4A1F-8302-63D831E8E7B6}");

		public struct Fields
		{
			public static readonly ID UserListsPage = new ID("{5C0CAE66-AE54-4C05-BE2D-1C6BE9F9B2B0}");
			public static readonly ID RegisterPage = new ID("{8226ED68-D939-4DB7-BBF3-D5F51475903B}");
			public static readonly ID LoginPage = new ID("{784BEF8A-47A9-48DC-B7BD-531C0D08DA83}");
			public static readonly ID AfterLoginPage = new ID("{83A33274-3188-4ACA-B634-DA2CBF8EDDA8}");
			public static readonly ID Error = new ID("{81C060E0-E2CB-49BA-B4FD-EFA7DE0989F6}");
		}
	}
}