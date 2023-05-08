using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MSIL.Startup))]
namespace MSIL
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			//ConfigureAuth(app);
		}
	}
}