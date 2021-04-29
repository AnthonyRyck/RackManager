using AccessData.Models;
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
		/// Change de page pour créer un nouveau client
		/// </summary>
		void OpenPageNewClient();



	}
}
