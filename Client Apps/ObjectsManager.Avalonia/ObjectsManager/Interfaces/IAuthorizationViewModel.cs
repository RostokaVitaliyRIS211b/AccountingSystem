using Avalonia.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.Interfaces
{
    public interface IAuthorizationViewModel
    {

		public string Name { get; set; }

		public string Password { get; set; }

		public string IpAddress { get; set; }

		public string Port { get; set; }

		public Window? Win { get; set; }
		public abstract Task Authorize();
	}
}
