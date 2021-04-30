using AccessData.Models;
using RackManager.ValidationModels;
using Radzen.Blazor;
using System.Collections.Generic;

namespace RackManager.ViewModels
{
	public interface IRackViewModel
	{
		/// <summary>
		/// Tous les racks
		/// </summary>
		List<Rack> AllRacks { get; set; }

		/// <summary>
		/// Indicateur si les données sont chargées.
		/// </summary>
		bool IsLoaded { get; set; }

		/// <summary>
		/// Indicateur s'il faut ouvrir le Dialog
		/// </summary>
		bool DialogIsOpenNewRack { get; set; }

		/// <summary>
		/// Model de validation pour un nouveau rack
		/// </summary>
		RackValidation NouveauRack { get; set; }

		/// <summary>
		/// Référence au tableau Radzen
		/// </summary>
		RadzenGrid<Rack> RackGrid { get; set; }

		/// <summary>
		/// Méthode pour valider le nouveau client
		/// </summary>
		void OnValidSubmit();

		/// <summary>
		/// Pour créer un nouveau rack
		/// </summary>
		void OpenNewRack();

		/// <summary>
		/// Ferme la vue du nouveau rack
		/// </summary>
		void CloseNouveauRack();
	}
}
