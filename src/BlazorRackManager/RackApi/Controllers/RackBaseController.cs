using AccessData;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Linq;
using System.Security.Claims;

namespace RackApi.Controllers
{
	public class RackBaseController : Controller
	{
		protected SqlContext ContextSql { get; private set; }

		private const string MESSAGE_API = "API - ";

		public RackBaseController(SqlContext sqlContext)
		{
			ContextSql = sqlContext;
		}
		
		/// <summary>
		/// Retourne le login de l'utilisateur.
		/// </summary>
		/// <returns></returns>
		protected string GetUserName()
		{
			return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
		}

		/// <summary>
		/// Log un message d'erreur avec son Exception
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="messageError"></param>
		protected void LogError(Exception exception, string messageError)
		{
			Log.Error(exception, MESSAGE_API + GetUserName() + " - " + messageError);
		}

		/// <summary>
		/// Log un message d'information
		/// </summary>
		/// <param name="message"></param>
		protected void LogInfo(string message)
		{
			Log.Information(MESSAGE_API + GetUserName() + " - " + message);
		}

		/// <summary>
		/// Log un message d'avertissement.
		/// </summary>
		/// <param name="messageWarn"></param>
		protected void LogWarning(string messageWarn)
		{
			Log.Warning(MESSAGE_API + GetUserName() + " - " + messageWarn);
		}
	}
}
