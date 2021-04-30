using AccessData.Models;
using RackManager.ValidationModels;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	interface IClientViewModel
	{
		/// <summary>
		/// Tous les clients
		/// </summary>
		List<Client> AllClients { get; set; }

		/// <summary>
		/// Indicateur si les données sont chargées.
		/// </summary>
		bool IsLoaded { get; set; }

		/// <summary>
		/// Indicateur s'il faut ouvrir le Dialog
		/// </summary>
		bool DialogIsOpenNewClient { get; set; }

		/// <summary>
		/// Model de validation pour un nouveau client
		/// </summary>
		ClientValidation NouveauClient { get; set; }

		/// <summary>
		/// Référence au tableau Radzen
		/// </summary>
		RadzenGrid<Client> ClientGrid { get; set; }

		/// <summary>
		/// Méthode pour valider le nouveau client
		/// </summary>
		void OnValidSubmit();

		/// <summary>
		/// Pour créer un nouveau client
		/// </summary>
		void OpenNewClient();

		/// <summary>
		/// Ferme la vue du nouveau client
		/// </summary>
		void CloseNouveauClient();

	}
}
