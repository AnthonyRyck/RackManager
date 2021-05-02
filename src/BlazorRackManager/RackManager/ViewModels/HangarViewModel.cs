using AccessData;
using AccessData.Models;
using AccessData.Views;
using Microsoft.AspNetCore.Components;
using RackManager.Composants;
using RackManager.ValidationModels;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public class HangarViewModel : IHangarViewModel
	{
		public bool IsLoaded { get; set; }

		public bool DeplacerPalette { get; set; }

		public Action StateChange { get; set; }

		public RenderFragment DisplayRenderFragment { get; set; }

		public List<HangarView> AllHangar { get; set; }

		public RadzenGrid<HangarView> HangarGrid { get; set; }

		public IEnumerable<Rack> Racks { get; set; }

		public IEnumerable<Rack> RacksFull { get; set; }

		public IEnumerable<Client> AllClients { get; set; }

		public EntreHangarValidation EntreHangarValidation { get; set; }

		public SortieHangarValidation SortieHangarValidation { get; set; }

		public TransfertRackValidation TransfertRackValidation { get; set; }

		public CommandeView ClientTransfert { get; set; }


		private SqlContext SqlContext;
		private NotificationService Notification;


		private GeoCommande nouvelleEntreHangar;

		public HangarViewModel(SqlContext sqlContext, NotificationService notification)
		{
			SqlContext = sqlContext;
			Notification = notification;
			IsLoaded = false;
			DeplacerPalette = false;

			TransfertRackValidation = new TransfertRackValidation();
			EntreHangarValidation = new EntreHangarValidation();
			SortieHangarValidation = new SortieHangarValidation();
			nouvelleEntreHangar = new GeoCommande();

			ClientTransfert = new CommandeView() 
			{ 
				NomClient = "Aucune sélection",
				DescriptionCmd = "aucune",
				IdClient = 0,
				IdCommande = 0
			};

			AllHangar = LoadDatas().GetAwaiter().GetResult();

			IsLoaded = true;
		}

		#region Public methods

		public void OpenTransfert()
		{
			DeplacerPalette = true;
		}

		public void CloseTransfert()
		{
			DeplacerPalette = false;
			TransfertRackValidation = new TransfertRackValidation();
		}

		public void SetStateHasChanged(Action stateHasChange)
		{
			StateChange = stateHasChange;
		}

		public async void OnValidTransfert()
		{
			try
			{
				await SqlContext.TransfertRackTo(TransfertRackValidation.IdRackPartant, TransfertRackValidation.IdRackArrivant);

				// Recharger les racks.
				Racks = await SqlContext.GetRackEmpty();
				RacksFull = await SqlContext.GetRackFull();

				AllHangar.RemoveAll(x => x.IdRack == TransfertRackValidation.IdRackPartant);
				HangarView hangar = await SqlContext.GetHangar(ClientTransfert.IdCommande, TransfertRackValidation.IdRackArrivant);
				AllHangar.Add(hangar);
				await HangarGrid.Reload();

				Notification.Notify(NotificationSeverity.Success, "Transfert OK", "Transfert effectué");

				// Remise à zéro
				TransfertRackValidation = new TransfertRackValidation();
				ClientTransfert = new CommandeView();
			}
			catch (Exception ex)
			{
				Notification.Notify(NotificationSeverity.Success, "Error", "Erreur sur le transfert");
			}
		}

		public async void OnSelectedRackPartant(object rackPartant)
		{
			var rackSelected = rackPartant as Rack;
			if (rackSelected != null)
			{
				TransfertRackValidation.IdRackPartant = rackSelected.IdRack;
				ClientTransfert = await SqlContext.GetCommandeByIdRack(rackSelected.IdRack);
			}
		}

		public void OnSelectedRackArrivant(object rackArrivant)
		{
			var rackSelected = rackArrivant as Rack;
			if (rackSelected != null)
				TransfertRackValidation.IdRackArrivant = rackSelected.IdRack;
		}


		#region Nouvelle entrée

		public void OpenNouvelleEntre()
		{
			RenderFragment CreateCompo() => builder =>
			{
				builder.OpenComponent(0, typeof(AjouterEntreHangar));
				builder.AddAttribute(1, "EntreHangarValidation", EntreHangarValidation);
				builder.AddAttribute(2, "AllClients", AllClients);
				builder.AddAttribute(3, "Racks", Racks);

				// Ajout pour EventCallBack
				var eventTerminerEntre = EventCallback.Factory.Create(this, CloseEntre);
				builder.AddAttribute(4, "OnTerminerClick", eventTerminerEntre);

				Action<Client> retourAction = OnSelectClient;
				var eventOnSelectClient = EventCallback.Factory.Create(this, retourAction);
				builder.AddAttribute(5, "OnSelectClient", eventOnSelectClient);

				Action<Rack> retourRack = OnSelectedRack;
				var eventOnSelectRack = EventCallback.Factory.Create(this, retourRack);
				builder.AddAttribute(6, "OnSelectedRack", eventOnSelectRack);

				var eventOnValidSubmitEntre = EventCallback.Factory.Create(this, OnValidSubmit);
				builder.AddAttribute(7, "OnValidSubmit", eventOnValidSubmitEntre);

				builder.CloseComponent();
			};

			DisplayRenderFragment = CreateCompo();
		}

		public async void OnValidSubmit()
		{
			try
			{
				nouvelleEntreHangar.DateEntree = EntreHangarValidation.DateEntree.Value;

				// Sauvegarde de la commande
				SuiviCommande cmd = EntreHangarValidation.ToSuiviCommande();
				await SqlContext.AddCommande(cmd);

				nouvelleEntreHangar.CommandeId = cmd.IdCommande;

				// Sauvegarde dans le hangar
				await SqlContext.AddToHangar(nouvelleEntreHangar);
				HangarView newEntry = await SqlContext.GetHangar(nouvelleEntreHangar.CommandeId, nouvelleEntreHangar.RackId);

				Notification.Notify(NotificationSeverity.Success, "Sauvegarde OK", "Sauvegarde OK");

				// remise à zéro
				nouvelleEntreHangar = new GeoCommande();
				EntreHangarValidation = new EntreHangarValidation();

				AllHangar.Add(newEntry);
				await HangarGrid.Reload();

				// Recharger les racks vides.
				Racks = await SqlContext.GetRackEmpty();

				StateChange.Invoke();
			}
			catch (Exception ex)
			{
				Notification.Notify(NotificationSeverity.Success, "Error", "Erreur sur la sauvegarde");
			}
		}

		public void OnSelectClient(Client client)
		{
			if (client != null)
			{
				EntreHangarValidation.IdClient = client.IdClient;
				EntreHangarValidation.NomClient = client.NomClient;
			}
		}

		public void OnSelectedRack(Rack rack)
		{
			if (rack != null)
				nouvelleEntreHangar.RackId = rack.IdRack;
		}

		public void CloseEntre()
		{
			DisplayRenderFragment = null;
			StateChange.Invoke();

			EntreHangarValidation = new EntreHangarValidation();
			nouvelleEntreHangar = new GeoCommande();
		}

		#endregion

		#region Sortie Hangar

		public void OpenSortie()
		{
			RenderFragment CreateCompo() => builder =>
			{
				builder.OpenComponent(0, typeof(SortieHangar));
				builder.AddAttribute(1, "RacksFull", RacksFull);
				builder.AddAttribute(2, "SortieHangarValidation", SortieHangarValidation);
				
				// Ajout pour EventCallBack
				var eventTerminerSortie = EventCallback.Factory.Create(this, CloseSortie);
				builder.AddAttribute(3, "CloseSortieHangar", eventTerminerSortie);

				Action<Rack> retourRack = OnSelectedRackSortie;
				var eventOnSelectRack = EventCallback.Factory.Create(this, retourRack);
				builder.AddAttribute(6, "OnSelectRackSortie", eventOnSelectRack);

				var eventOnValidSubmitSortie = EventCallback.Factory.Create(this, OnValidSortieSubmit);
				builder.AddAttribute(7, "OnValidSortieSubmit", eventOnValidSubmitSortie);

				builder.CloseComponent();
			};

			DisplayRenderFragment = CreateCompo();
		}

		public async void OnValidSortieSubmit()
		{
			try
			{
				// enlever de geocommande, la palette
				await SqlContext.DeleteToHangar(SortieHangarValidation.IdRack, SortieHangarValidation.IdCommande.Value);

				// mettre la commande avec une date de sortie
				await SqlContext.UpdateSortieCommande(SortieHangarValidation.IdCommande.Value, SortieHangarValidation.DateSortie.Value);

				AllHangar.RemoveAll(x => x.IdCommande == SortieHangarValidation.IdCommande.Value
										&& x.IdRack == SortieHangarValidation.IdRack);
				await HangarGrid.Reload();

				Notification.Notify(NotificationSeverity.Success, "Sortie OK", "Sortie OK");
				// remise à zéro
				SortieHangarValidation = new SortieHangarValidation();

				// Recharger les racks.
				Racks = await SqlContext.GetRackEmpty();
				RacksFull = await SqlContext.GetRackFull();

				StateChange.Invoke();
			}
			catch (Exception ex)
			{
				Notification.Notify(NotificationSeverity.Success, "Error", "Erreur sur la sauvegarde");
			}
		}

		public void OnSelectedRackSortie(Rack rackSelected)
		{
			if (rackSelected != null)
				SortieHangarValidation.IdRack = rackSelected.IdRack;
		}

		public void CloseSortie()
		{
			DisplayRenderFragment = null;
			StateChange.Invoke();

			SortieHangarValidation = new SortieHangarValidation();
		}

		#endregion

		#endregion

		#region Private methods

		private async Task<List<HangarView>> LoadDatas()
		{
			List<HangarView> hangarViews = new List<HangarView>();
			Racks = new List<Rack>();
			AllClients = new List<Client>();
			RacksFull = new List<Rack>();

			try
			{
				hangarViews = await SqlContext.GetHangar();
				Racks = await SqlContext.GetRackEmpty();
				RacksFull = await SqlContext.GetRackFull();
				AllClients = await SqlContext.LoadClients();
			}
			catch (Exception ex)
			{
				Notification.Notify(NotificationSeverity.Error, "Erreur chargement", "Erreur sur le chargement des informations du hangar");
			}

			return hangarViews;
		}

		#endregion
	}
}
