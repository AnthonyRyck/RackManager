using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RackApi.Data;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace RackApi
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

			//Log.Logger = new LoggerConfiguration()
			//	.MinimumLevel.Debug()
			//	.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
			//	.MinimumLevel.Override("System", LogEventLevel.Warning)
			//	.WriteTo.RollingFile(Path.Combine(pathLog, "log-{Date}.txt"))
			//	.CreateLogger();

			// Pour les logs.
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
							.WriteTo.MySQL(connectionDb, "Logs")
							.CreateLogger();

			try
			{
				CreateHostBuilder(args).Build().Run();
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
