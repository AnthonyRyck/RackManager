using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackApi.SecureApi
{
	public interface IJwtAuthenticationManager
	{
		Task<string> Authenticate(string login, string password);
	}
}
