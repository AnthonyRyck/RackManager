using AccessData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using RackApi.Areas.Identity;
using RackApi.Data;
using RackApi.SecureApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RackApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			string connectionDb = Configuration.GetConnectionString("MySqlConnection");

			// *** Dans le cas ou une utilisation avec DOCKER
			// *** voir post sur : https://www.ctrl-alt-suppr.dev/2021/02/01/connectionstring-et-image-docker/
			string databaseAddress = Environment.GetEnvironmentVariable("DB_HOST");
			string login = Environment.GetEnvironmentVariable("LOGIN_DB");
			string mdp = Environment.GetEnvironmentVariable("PASSWORD_DB");
			string dbName = Environment.GetEnvironmentVariable("DB_NAME");

			connectionDb = connectionDb.Replace("USERNAME", login)
									.Replace("YOURPASSWORD", mdp)
									.Replace("YOURDB", dbName)
									.Replace("YOURDATABASE", databaseAddress);

			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseMySql(connectionDb, ServerVersion.AutoDetect(connectionDb)));

			services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddRazorPages();
			services.AddServerSideBlazor();
			services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
			services.AddDatabaseDeveloperPageExceptionFilter();

			// Service SQL de AccessData.
			services.AddSingleton(new SqlContext(connectionDb));

			// Pour JWT auth
			var key = Configuration["KeyPourJwt"];

			services.AddAuthentication(x =>
			{
				// Si ajout de cette ligne, plus possible de s'authentifier sur le site.
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(x =>
				{
					x.RequireHttpsMetadata = true;
					x.SaveToken = true;
					x.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
						ValidateIssuer = false,
						ValidateAudience = false,
					};
				});

			services.AddAuthorization(options =>
			{
				options.AddPolicy("RequestAdmin", policy =>
					   policy.RequireRole("Admin"));

				options.AddPolicy("ManagerRequest", policy =>
					   policy.RequireRole("Manager"));
			});

			services.AddScoped<IJwtAuthenticationManager, JwtAuthenticationManager>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapBlazorHub();
				endpoints.MapFallbackToPage("/_Host");
			});

			// Pour forcer l'application en Français.
			var cultureInfo = new CultureInfo("fr-Fr");
			cultureInfo.NumberFormat.CurrencySymbol = "€";

			CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
			CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
		}
	}
}
