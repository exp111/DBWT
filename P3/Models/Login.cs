using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace P3.Models
{
	public class Login
	{
		public bool LoggedIn { get; set; }
		public string Username { get; set; }
		public string Role { get; set; }
		public bool Failed { get; set; }
		public int ID { get; set; }
		public string Salt { get; set; }
		public string Hash { get; set; }
	}
}