using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace RackMobile.Services
{
	public interface IRackService
	{
		Task<bool> TestServerUrl(string adresseServer);
		void ChangeServerAddress(string addressServer);

	}
}
