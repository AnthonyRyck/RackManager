using AccessData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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

			try
			{
				var host = CreateHostBuilder(args).Build();

				var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
				using (var scope = scopeFactory.CreateScope())
				{
					var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
					var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
					var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
					var sqlContext = scope.ServiceProvider.GetRequiredService<SqlContext>();

					// Vrai si la base de donn�es est cr��e, false si elle existait d�j�.
					if (db.Database.EnsureCreated())
					{
						DataInitializer.InitData(roleManager, userManager).Wait();

						// cr�er le reste de la base
						DataInitializer.CreateTables(sqlContext).Wait();
					}
				}
				
				// Pour les logs.
				// ATTENTION : il faut que la table Logs (cr�� par Serilog) soit faites APRES
				// la cr�ation des tables ASP, sinon "db.Database.EnsureCreated" consid�re que la
				// base est d�j� cr��e.
				var configuration = new ConfigurationBuilder()
							.SetBasePath(Directory.GetCurrentDirectory())
							.AddJsonFile("appsettings.json")
							.Build();

				string connectionDb = configuration.GetConnectionString("MySqlConnection");

				string databaseAddress = Environment.GetEnvironmentVariable("DB_HOST");
				string login = Environment.GetEnvironmentVariable("LOGIN_DB");
				string mdp = Environment.GetEnvironmentVariable("PASSWORD_DB");
				string dbName = Environment.GetEnvironmentVariable("DB_NAME");

				connectionDb = connectionDb.Replace("USERNAME", login)
										.Replace("YOURPASSWORD", mdp)
										.Replace("YOURDB", dbName)
										.Replace("YOURDATABASE", databaseAddress);

				Log.Logger = new LoggerConfiguration()
					.MinimumLevel.Debug()
					.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
					.MinimumLevel.Override("System", LogEventLevel.Warning)
					.WriteTo.RollingFile(Path.Combine(pathLog, "log-{Date}.txt"))
					.WriteTo.MySQL(connectionDb, "Logs")
					.CreateLogger();

				host.Run();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Erreur dans Main");
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
