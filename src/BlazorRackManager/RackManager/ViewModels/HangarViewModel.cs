using AccessData;
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

		public List<HangarView> AllHangar { get; set; }

		public RadzenGrid<HangarView> HangarGrid { get; set; }

		public IEnumerable<Rack> Racks { get; set; }

		public IEnumerable<Client> AllClients { get; set; }

		public EntreHangarValidation EntreHangarValidation { get; set; }

		private SqlContext SqlContext;
		private NotificationService Notification;


		private GeoCommande nouvelleEntreHangar;

		public HangarViewModel(SqlContext sqlContext, NotificationService notification)
		{
			SqlContext = sqlContext;
			Notification = notification;
			IsLoaded = false;
			DialogNouvelleEntre = false;

			EntreHangarValidation = new EntreHangarValidation();
			nouvelleEntreHangar = new GeoCommande();

			AllHangar = LoadDatas().GetAwaiter().GetResult();

			IsLoaded = true;
		}

		#region Public methods

		public void OpenNouvelleEntre()
		{
			DialogNouvelleEntre = true;
		}

		public void CloseEntre()
		{
			DialogNouvelleEntre = false;
			EntreHangarValidation = new EntreHangarValidation();
			nouvelleEntreHangar = new GeoCommande();
		}


		public void OnSelectedRack(object selected)
		{
			var rackSelected = selected as Rack;
			if (rackSelected != null)
				nouvelleEntreHangar.RackId = rackSelected.IdRack;
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


		public void OnSelectClient(object client)
		{
			Client clientSelected = client as Client;

			if (clientSelected != null)
			{
				EntreHangarValidation.IdClient = clientSelected.IdClient;
				EntreHangarValidation.NomClient = clientSelected.NomClient;
			}
		}

		#endregion

		#region Private methods

		private async Task<List<HangarView>> LoadDatas()
		{
			List<HangarView> hangarViews = new List<HangarView>();
			Racks = new List<Rack>();
			AllClients = new List<Client>();

			try
			{
				hangarViews = await SqlContext.GetHangar();
				Racks = await SqlContext.GetRackEmpty();
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
