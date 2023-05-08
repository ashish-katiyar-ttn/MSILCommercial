using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSIL.Models
{
	public class CreateUser
	{
		public string Domain { get; set; } = "sitecore";
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public string Role { get; set; }
		public List<RoleList> RolesList { get; set; }
	}
	public class RoleList
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}
}