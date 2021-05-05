using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessData.Views;
using Radzen.Blazor;

namespace RackManager.ViewModels
{
	public interface ISortiesViewModel
	{
		bool IsLoaded { get; set; }

		IEnumerable<CommandeSortieView> Sorties { get; set; }

		RadzenGrid<CommandeSortieView> SortieGrid { get; set; }
	}
}
