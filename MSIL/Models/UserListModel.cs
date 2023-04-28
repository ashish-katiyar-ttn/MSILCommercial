using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSIL.Models
{
	public class UserListModel
	{
		public string Username { get; set; }
		public List<UserList> userList {  get; set; }
		public List<RoleLists> roleList { get; set; }
	}
}