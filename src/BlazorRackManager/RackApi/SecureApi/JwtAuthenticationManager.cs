using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace RackApi.SecureApi
{
	public class JwtAuthenticationManager : IJwtAuthenticationManager
	{
		private string mySuperKey;
		private readonly UserManager<IdentityUser> userManager;

		public JwtAuthenticationManager(UserManager<IdentityUser> user, IConfiguration configuration)
		{
			userManager = user;
			mySuperKey = configuration["KeyPourJwt"];
		}


		public async Task<string> Authenticate(string login, string password)
		{
			var user = await userManager.FindByNameAsync(login);

			// Si utilisateur n'existe pas.
			if (user == null)
				return null;

			// Si mauvais mot de passe
			if (!await userManager.CheckPasswordAsync(user, password))
				return null;

			var rolesUser = await userManager.GetRolesAsync(user);
			string role = rolesUser.First();

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenKey = Encoding.ASCII.GetBytes(mySuperKey);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name, login),
					new Claim(ClaimTypes.Role, role)
				}),
				Expires = DateTime.UtcNow.AddHours(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
																SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}

	}
}
