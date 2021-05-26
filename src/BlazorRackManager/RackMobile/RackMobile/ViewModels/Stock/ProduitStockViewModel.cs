using RackCore.EntityView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RackMobile.ViewModels.Stock
{
	public class ProduitStockViewModel : BaseViewModel
	{
		/// <summary>
		/// Liste des racks contenants l'informations.
		/// </summary>
		public List<StockView> Racks
		{
			get { return _racks; }
			set
			{
				_racks = value;
				OnNotifyPropertyChanged();
			}
		}
		private List<StockView> _racks;

		/// <summary>
		/// Charge les racks STOCK pour cette référence de produit.
		/// </summary>
		/// <returns></returns>
		internal async Task GetStockByProduit(string refProduit)
		{
			try
			{
				Racks = await RackService.GetRackStock(refProduit);
			}
			catch (UnauthorizedAccessException)
			{
				MessageInformation = "Connexion non autorisé";
			}
			catch (Exception)
			{
				MessageInformation = "Une erreur s'est produite";
			}
		}
	}
}
