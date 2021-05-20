using RackCore;
using AccessData.Views;
using Microsoft.AspNetCore.Components;
using RackManager.ValidationModels;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RackCore.EntityView;

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

		Task LoadDatas();


		void OnSelectedRack(string rack);

		void OnSelectedRackSortie(string rackSelected);

		void OpenNouvelleEntre();

		void OpenSortie();

		void OpenTransfert();

		void OnValidSubmit();

		void OnValidSortieSubmit();

		void OnValidTransfert();

		void OnSelectClient(Client client);



		void OnSelectedRackPartant(string rackPartant);

		void OnSelectedRackArrivant(string rackArrivant);



		void OpenIntervertir();
	}
}
