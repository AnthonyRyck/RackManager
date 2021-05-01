using AccessData.Models;
using AccessData.Views;
using RackManager.ValidationModels;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public interface IHangarViewModel
	{
		bool IsLoaded { get; set; }

		bool DialogNouvelleEntre { get; set; }

		List<HangarView> AllHangar { get; set; }

		IEnumerable<Client> AllClients { get; set; }

		RadzenGrid<HangarView> HangarGrid { get; set; }

		EntreHangarValidation EntreHangarValidation { get; set; }

		IEnumerable<Rack> Racks { get; set; }

		IEnumerable<SuiviCommande> Commandes { get; set; }


		void OnSelectedRack(object selected);


		void OpenNouvelleEntre();

		void CloseEntre();

		void OnValidSubmit();

		void OnSelectClient(object client);
	}
}
