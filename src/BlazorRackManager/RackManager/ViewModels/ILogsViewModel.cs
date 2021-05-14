using Microsoft.AspNetCore.Components;
using RackCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public interface ILogsViewModel
	{
		List<LogEntity> Logs { get; }

		DateTime DateDebutLog { get; set; }

		void OnChangeLevel(ChangeEventArgs e);


		Task LoadLogs();
	}
}
