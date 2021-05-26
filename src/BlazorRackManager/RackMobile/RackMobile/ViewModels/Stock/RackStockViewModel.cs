using RackCore.EntityView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackMobile.ViewModels.Stock
{
	public class RackStockViewModel : BaseViewModel
	{
		/// <summary>
		/// Liste des racks contenant du Stock, pour l'affichage
		/// </summary>
		public List<StockView> RacksStock
		{
			get { return _racksStock; }
			set
			{
				_racksStock = value;
				OnNotifyPropertyChanged();
			}
		}
		private List<StockView> _racksStock;

		/// <summary>
		/// Liste de tous les racks contenant du stock
		/// </summary>
		private List<StockView> AllRackStock;

		internal async Task Init()
		{
			try
			{
				AllRackStock = await RackService.GetRackStock();
				RacksStock = AllRackStock.ToList();
			}
			catch (UnauthorizedAccessException)
			{
				MessageInformation = "Vous n'êtes pas autorisé à accèder à cette ressource."
							+ Environment.NewLine
							+ "Voir pour remettre votre login et mot de passe.";
			}
			catch (Exception)
			{
				MessageInformation = "Erreur sur le chargement des racks";
			}
		}

		/// <summary>
		/// Filtre la liste par rapport au gisement donnée.
		/// </summary>
		/// <param name="gisement"></param>
		internal void Filtrer(string gisement)
		{
			RacksStock = AllRackStock.Where(x => x.GisementPos.Contains(gisement)).ToList();
		}

	}
}
