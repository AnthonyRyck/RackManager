using AccessData.Models;
using AccessData.Views;
using Microsoft.AspNetCore.Components;
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

		List<HangarView> AllHangar { get; set; }

		IEnumerable<Client> AllClients { get; set; }

		RadzenGrid<HangarView> HangarGrid { get; set; }

		EntreHangarValidation EntreHangarValidation { get; set; }

		SortieHangarValidation SortieHangarValidation { get; set; }

		TransfertRackValidation TransfertRackValidation { get; set; }

		IEnumerable<Rack> Racks { get; set; }

		IEnumerable<Rack> RacksFull { get; set; }

		CommandeView ClientTransfert { get; set; }

		RenderFragment DisplayRenderFragment { get; set; }


		Action StateChange { get; set; }

		void SetStateHasChanged(Action stateHasChange);




		void OnSelectedRack(Rack rack);

		void OnSelectedRackSortie(Rack rackSelected);

		void OpenNouvelleEntre();

		void OpenSortie();

		void OpenTransfert();

		void CloseTransfert();

		void CloseEntre();

		void CloseSortie();

		void OnValidSubmit();

		void OnValidSortieSubmit();

		void OnValidTransfert();

		void OnSelectClient(Client client);



		void OnSelectedRackPartant(object rackPartant);

		void OnSelectedRackArrivant(object rackArrivant);
	}
}
