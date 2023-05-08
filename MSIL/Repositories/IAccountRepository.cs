using Sitecore.Security.Accounts;
namespace MSIL.Repositories
{
	public interface IAccountRepository
	{
		User Login(string userName, string password);
	}
}
