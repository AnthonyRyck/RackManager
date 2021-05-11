using Microsoft.AspNetCore.Mvc;
using RackCore;
using RackApi.SecureApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class MyConnectController : Controller
	{
		private readonly IJwtAuthenticationManager jwtAuthenticationManager;

		public MyConnectController(IJwtAuthenticationManager jwtAuthentication)
		{
			jwtAuthenticationManager = jwtAuthentication;
		}

		[HttpPost("authenticate")]
		public async Task<IActionResult> Authenticate([FromBody] UserCredential userCred)
		{
			var token = await jwtAuthenticationManager.Authenticate(userCred.Login, userCred.Password);
			if (token == null)
				return Unauthorized();

			return Ok(token);
		}

		/// <summary>
		/// Permet de tester la connection avec le serveur.
		/// </summary>
		/// <returns></returns>
		[HttpGet("testconnect")]
		public string GetTestConnection()
		{
			return "Connexion OK";
		}

	}
}
