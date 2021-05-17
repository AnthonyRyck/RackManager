using RackCore;
using RackManager.ValidationModels;
using Radzen.Blazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	interface IUniteMesureViewModel
	{
		/// <summary>
		/// Indicateur si la page est chargé
		/// </summary>
		bool IsLoaded { get; }

		/// <summary>
		/// Indicateur s'il faut ouvrir un panel pour
		/// une nouvelle unité mesure
		/// </summary>
		bool CanOpenNewUnite { get; }

		/// <summary>
		/// Model de validation pour une mesure
		/// </summary>
		UniteMesureValidation UniteMesure { get; }

		/// <summary>
		/// Liste de toutes les mesures
		/// </summary>
		List<UniteMesure> AllMesures { get; }

		/// <summary>
		/// Référence au tableau Radzen
		/// </summary>
		RadzenGrid<UniteMesure> UniteGrid { get; set; }

		/// <summary>
		/// Charge toutes les unités de mesure
		/// </summary>
		/// <returns></returns>
		Task LoadUnites();

		/// <summary>
		/// Pour ouvrir le panel d'une nouvelle unité
		/// de mesure
		/// </summary>
		void OpenNewUnite();

		/// <summary>
		/// Ferme le panel.
		/// </summary>
		void CloseNewMesure();

		/// <summary>
		/// Méthode pour la validation d'une nouvelle
		/// mesure.
		/// </summary>
		/// <returns></returns>
		Task OnValidSubmitMesure();
	}
}
