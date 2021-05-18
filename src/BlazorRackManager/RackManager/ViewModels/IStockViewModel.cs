using AccessData.Views;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public interface IStockViewModel
	{
		bool IsLoaded { get; set; }

		List<StockView> AllStock { get; set; }

		
		RenderFragment DisplayRenderFragment { get; set; }

		RadzenGrid<StockView> StockGrid { get; set; }


		void SetStateHasChanged(Action stateHasChange);
		Task LoadStocks();

		void OpenNouvelleEntre();
	}
}
