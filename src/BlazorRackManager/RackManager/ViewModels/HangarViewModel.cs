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

		public IEnumerable<SuiviCommande> Commandes { get; set; }

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

		public void OnSelectCommande(object selected)
		{
			var commande = selected as SuiviCommande;
			if(commande != null)
				nouvelleEntreHangar.CommandeId = commande.IdCommande;
		}

		public async void OnValidSubmit()
		{
			try
			{
				nouvelleEntreHangar.DateEntree = EntreHangarValidation.DateEntree.Value;

				await SqlContext.AddToHangar(nouvelleEntreHangar);
				HangarView newEntry = await SqlContext.GetHangar(nouvelleEntreHangar.CommandeId, nouvelleEntreHangar.RackId);

				Notification.Notify(NotificationSeverity.Success, "Sauvegarde OK", "Sauvegarde OK");
				
				nouvelleEntreHangar = new GeoCommande();
				EntreHangarValidation = new EntreHangarValidation();

				AllHangar.Add(newEntry);
				await HangarGrid.Reload();

				// Recharger les racks vide.
				Racks = await SqlContext.GetRackEmpty();
			}
			catch (Exception ex)
			{
				Notification.Notify(NotificationSeverity.Success, "Error", "Erreur sur la sauvegarde");
			}
		}

		#endregion

		#region Private methods

		private async Task<List<HangarView>> LoadDatas()
		{
			List<HangarView> hangarViews = new List<HangarView>();
			Racks = new List<Rack>();
			Commandes = new List<SuiviCommande>();

			try
			{
				hangarViews = await SqlContext.GetHangar();
				Racks = await SqlContext.GetRackEmpty();
				Commandes = await SqlContext.GetCommandes();
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
