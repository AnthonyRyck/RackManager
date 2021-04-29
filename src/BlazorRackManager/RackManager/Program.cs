using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RackManager.Data;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace RackManager
{
	public class Program
	{
		public static void Main(string[] args)
		{
			string pathLog = Path.Combine(AppContext.BaseDirectory, "Logs");
			if (!Directory.Exists(pathLog))
			{
				Directory.CreateDirectory(pathLog);
			}

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.MinimumLevel.Override("System", LogEventLevel.Warning)
				.WriteTo.RollingFile(Path.Combine(pathLog, "log-{Date}.txt"))
				.CreateLogger();

			try
			{
				var host = CreateHostBuilder(args).Build();

				var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
				using (var scope = scopeFactory.CreateScope())
				{
					var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
					var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
					var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

					// Vrai si la base de donn�es est cr��e, false si elle existait d�j�.
					if (db.Database.EnsureCreated())
					{
						DataInitializer.InitData(roleManager, userManager).Wait();
					}
				}

				host.Run();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Erreur dans Main");
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
