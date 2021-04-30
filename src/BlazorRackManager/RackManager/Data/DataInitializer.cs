using AccessData;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.Data
{
	public class DataInitializer
	{
		public static async Task InitData(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
		{
			var roles = Enum.GetNames(typeof(Role));

			foreach (var role in roles)
			{
				// User est juste pour l'affichage.
				if (role == Role.SansRole.ToString())
					continue;

				if (!await roleManager.RoleExistsAsync(role))
				{
					await roleManager.CreateAsync(new IdentityRole(role));
				}
			}

			// Création de l'utilisateur Root.
			var user = await userManager.FindByNameAsync("root");

			if (user == null)
			{
				var poweruser = new IdentityUser
				{
					UserName = "root",
					Email = "root@email.com",
					EmailConfirmed = true
				};
				string userPwd = "Azerty123!";
				var createPowerUser = await userManager.CreateAsync(poweruser, userPwd);
				if (createPowerUser.Succeeded)
				{
					await userManager.AddToRoleAsync(poweruser, Role.Admin.ToString());
				}
			}
		}


		internal static async Task CreateTables(SqlContext sqlContext)
		{
			try
			{
				string pathSql = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", "CreateDb.sql");
				await sqlContext.CreateTables(pathSql);
			}
			catch (Exception ex)
			{
				//Log.Error(ex, "Erreur sur la création de la base de donnée.");
			}
		}

	}
}
