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


		decimal? MaxQuantite { get; }

		decimal? MinQuantite { get; }

		bool RowOnUpdate { get; }

		#region Event DataGrid

		/// <summary>
		/// Sauvegarde en BDD des modifications
		/// </summary>
		/// <param name="currentSalle"></param>
		void OnUpdateRow(StockView currentStock);

		void EditRow(StockView stock, bool isAdd);

		void SaveRow(StockView stock);

		void CancelEdit(StockView currentStock);

		#endregion
	}
}
