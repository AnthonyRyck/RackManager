using AccessData.Models;
using AccessData.Views;
using RackManager.ValidationModels;
using Radzen.Blazor;
using System.Collections.Generic;

namespace RackManager.ViewModels
{
	public interface ICommandeViewModel
	{
		/// <summary>
		/// Tous les commandes
		/// </summary>
		List<CommandeView> AllCommandes { get; set; }

		/// <summary>
		/// Tous les clients
		/// </summary>
		IEnumerable<Client> AllClients { get; set; }

		/// <summary>
		/// Indicateur si les données sont chargées.
		/// </summary>
		bool IsLoaded { get; set; }

		/// <summary>
		/// Indicateur s'il faut ouvrir le Dialog
		/// </summary>
		bool DialogIsOpenNewCommande { get; set; }

		/// <summary>
		/// Model de validation pour une nouvelle commande
		/// </summary>
		CommandeValidation NouvelleCommande { get; set; }

		/// <summary>
		/// Référence au tableau Radzen
		/// </summary>
		RadzenGrid<CommandeView> CommandeGrid { get; set; }

		/// <summary>
		/// Méthode pour valider la nouvelle commande
		/// </summary>
		void OnValidSubmit();

		/// <summary>
		/// Pour créer une nouvelle commande
		/// </summary>
		void OpenNewCommande();

		/// <summary>
		/// Ferme la vue de la nouvelle commande
		/// </summary>
		void CloseNouvelleCommande();


		void OnSelectClient(object client);
	}
}
