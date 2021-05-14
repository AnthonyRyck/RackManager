using AccessData;
using Microsoft.AspNetCore.Components;
using RackCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public class LogsViewModel : ILogsViewModel
	{
		#region Properties


		public List<LogEntity> Logs { get; private set; }

		public DateTime DateDebutLog { get; set; }



		#endregion


		private SqlContext ContextSql;
		private string LevelSelected;

		public LogsViewModel(SqlContext sqlContext)
		{
			ContextSql = sqlContext;

			LevelSelected = "Information";
			DateDebutLog = DateTime.Now.AddDays(-5);
		}


		public void OnChangeLevel(ChangeEventArgs e)
		{
			LevelSelected = e.Value.ToString();
		}


		public async Task LoadLogs()
		{
			try
			{
				Logs = await ContextSql.GetLogs(LevelSelected, DateDebutLog);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Erreur dans LoadLogs");
			}
		}

	}
}
