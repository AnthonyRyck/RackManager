﻿using AccessData;
using AccessData.Models;
using AccessData.Views;
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

		public bool DialogNouvelleEntre { get; set; }

		public bool DialogSortie { get; set; }

		public bool DeplacerPalette { get; set; }

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
			DialogNouvelleEntre = false;
			DialogSortie = false;
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

		public void OpenNouvelleEntre()
		{
			DialogSortie = false;
			DeplacerPalette = false;
			DialogNouvelleEntre = true;
		}

		public void OpenSortie()
		{
			DialogNouvelleEntre = false;
			DeplacerPalette = false;
			DialogSortie = true;
		}

		public void CloseEntre()
		{
			DialogNouvelleEntre = false;
			EntreHangarValidation = new EntreHangarValidation();
			nouvelleEntreHangar = new GeoCommande();
		}

		public void OpenTransfert()
		{
			DialogSortie = false;
			DialogNouvelleEntre = false;
			DeplacerPalette = true;
		}

		public void CloseTransfert()
		{
			DeplacerPalette = false;
			TransfertRackValidation = new TransfertRackValidation();
		}

		public void CloseSortie()
		{
			DialogSortie = false;
		}

		public void OnSelectedRack(object selected)
		{
			var rackSelected = selected as Rack;
			if (rackSelected != null)
				nouvelleEntreHangar.RackId = rackSelected.IdRack;
		}

		public void OnSelectedRackSortie(object rack)
		{
			var rackSelected = rack as Rack;
			if (rackSelected != null)
				SortieHangarValidation.IdRack = rackSelected.IdRack;
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
			}
			catch (Exception ex)
			{
				Notification.Notify(NotificationSeverity.Success, "Error", "Erreur sur la sauvegarde");
			}
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
			}
			catch (Exception ex)
			{
				Notification.Notify(NotificationSeverity.Success, "Error", "Erreur sur la sauvegarde");
			}
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

		public void OnSelectClient(object client)
		{
			Client clientSelected = client as Client;

			if (clientSelected != null)
			{
				EntreHangarValidation.IdClient = clientSelected.IdClient;
				EntreHangarValidation.NomClient = clientSelected.NomClient;
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
