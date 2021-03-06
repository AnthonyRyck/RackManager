using AccessData;
using RackCore;
using AccessData.Views;
using Microsoft.AspNetCore.Components;
using RackManager.Composants;
using RackManager.ValidationModels;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RackCore.EntityView;
using Serilog;
using System.Linq;

namespace RackManager.ViewModels
{
	public class HangarViewModel : IHangarViewModel
	{
		public bool IsLoaded { get; set; }
		
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

		private InversionPaletteValidation IntervertirValidation;

		public CommandeView ClientTransfert { get; set; }


		private SqlContext SqlContext;
		private NotificationService Notification;


		public HangarViewModel(SqlContext sqlContext, NotificationService notification)
		{
			SqlContext = sqlContext;
			Notification = notification;
		}

		#region Public methods

		public async Task LoadDatas()
		{
			IsLoaded = false;

			ClientTransfert = new CommandeView()
			{
				NomClient = "Aucune sélection",
				DescriptionCmd = "aucune",
				IdClient = 0,
				IdCommande = 0
			};

			Racks = new List<Rack>();
			AllClients = new List<Client>();
			RacksFull = new List<Rack>();

			TransfertRackValidation = new TransfertRackValidation();
			EntreHangarValidation = new EntreHangarValidation();
			SortieHangarValidation = new SortieHangarValidation();
			IntervertirValidation = new InversionPaletteValidation();

			try
			{
				AllHangar = await SqlContext.GetHangar();
				Racks = await SqlContext.GetRackEmpty();
				RacksFull = await SqlContext.GetRackFull();
				AllClients = await SqlContext.LoadClients();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "HangarViewModel - LoadDatas");
				Notification.Notify(NotificationSeverity.Error, "Erreur chargement", "Erreur sur le chargement des informations du hangar");
			}

			IsLoaded = true;
		}

		public void SetStateHasChanged(Action stateHasChange)
		{
			StateChange = stateHasChange;
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

				Action<string> retourRack = OnSelectedRack;
				var eventOnSelectRack = EventCallback.Factory.Create(this, retourRack);
				builder.AddAttribute(6, "OnSelectedRack", eventOnSelectRack);

				var eventOnValidSubmitEntre = EventCallback.Factory.Create(this, OnValidSubmit);
				builder.AddAttribute(7, "OnValidSubmit", eventOnValidSubmitEntre);

				builder.CloseComponent();
			};

			DisplayRenderFragment = CreateCompo();
		}

		public void CloseEntre()
		{
			DisplayRenderFragment = null;
			StateChange.Invoke();

			EntreHangarValidation = new EntreHangarValidation();
		}

		public async void OnValidSubmit()
		{
			try
			{
				if (EntreHangarValidation.IdRack == 0)
				{
					Notification.Notify(NotificationSeverity.Warning, "Attention", "Le Rack choisi n'est pas bon.");
					return;
				}

				// Sauvegarde de la commande
				SuiviCommande cmd = EntreHangarValidation.ToSuiviCommande();
				await SqlContext.AddCommande(cmd);

				// Sauvegarde dans le hangar
				GeoCommande nouvelleEntreHangar = EntreHangarValidation.ToGeoCommande();
				await SqlContext.AddToHangar(nouvelleEntreHangar);

				HangarView newEntry = await SqlContext.GetHangar(nouvelleEntreHangar.CommandeId, nouvelleEntreHangar.RackId);

				Notification.Notify(NotificationSeverity.Success, "Sauvegarde OK", "Sauvegarde OK");
				Log.Information("HANGAR ENTREE - {date} : commande- {commande} - Gisement-{rack}",
										nouvelleEntreHangar.DateEntree.ToString("d"),
										cmd.IdCommande, 
										EntreHangarValidation.GisementRack);

				// remise à zéro
				EntreHangarValidation = new EntreHangarValidation();

				AllHangar.Add(newEntry);
				await HangarGrid.Reload();

				// Recharger les racks vides.
				Racks = await SqlContext.GetRackEmpty();

				StateChange.Invoke();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "HangarViewModel - OnValidSubmit");
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

		public void OnSelectedRack(string selected)
		{
			SelectedRackEmpty(selected, EntreHangarValidation);
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

				Action<string> retourRack = OnSelectedRackSortie;
				var eventOnSelectRack = EventCallback.Factory.Create(this, retourRack);
				builder.AddAttribute(6, "OnSelectRackSortie", eventOnSelectRack);

				var eventOnValidSubmitSortie = EventCallback.Factory.Create(this, OnValidSortieSubmit);
				builder.AddAttribute(7, "OnValidSortieSubmit", eventOnValidSubmitSortie);

				builder.CloseComponent();
			};

			DisplayRenderFragment = CreateCompo();
		}

		public void CloseSortie()
		{
			DisplayRenderFragment = null;
			StateChange.Invoke();

			SortieHangarValidation = new SortieHangarValidation();
		}


		public async void OnValidSortieSubmit()
		{
			try
			{
				if (SortieHangarValidation.IdRack == 0)
				{
					Notification.Notify(NotificationSeverity.Warning, "Attention", "Le Rack choisi n'est pas bon.");
					return;
				}

				// enlever de geocommande, la palette
				await SqlContext.DeleteToHangar(SortieHangarValidation.IdRack, SortieHangarValidation.IdCommande.Value);

				// mettre la commande avec une date de sortie
				await SqlContext.UpdateSortieCommande(SortieHangarValidation.IdCommande.Value, SortieHangarValidation.DateSortie.Value);

				Log.Information("HANGAR SORTIE - {date} : commande- {commande} - Gisement-{rack}",
										SortieHangarValidation.DateSortie.Value.ToString("d"),
										SortieHangarValidation.IdCommande.Value,
										SortieHangarValidation.GisementRack);

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
				Log.Error(ex, "HangarViewModel - OnValidSortieSubmit");
				Notification.Notify(NotificationSeverity.Error, "Error", "Erreur sur la sauvegarde");
			}
		}

		public void OnSelectedRackSortie(string rackSelected)
		{
			SelectedRackFull(rackSelected, SortieHangarValidation);
		}

		#endregion

		#region Déplacer palette

		public void OpenTransfert()
		{
			RenderFragment CreateCompo() => builder =>
			{
				builder.OpenComponent(0, typeof(DeplacerPalette));

				var eventOnValidSubmitDeplacer = EventCallback.Factory.Create(this, OnValidTransfert);
				builder.AddAttribute(1, "OnValidTransfert", eventOnValidSubmitDeplacer);
				builder.AddAttribute(2, "TransfertRackValidation", TransfertRackValidation);
				builder.AddAttribute(3, "RackFull", RacksFull);
				builder.AddAttribute(4, "RackEmpty", Racks);

				var eventTerminerTransfert = EventCallback.Factory.Create(this, CloseTransfert);
				builder.AddAttribute(5, "CloseTransfert", eventTerminerTransfert);

				builder.AddAttribute(6, "ClientTransfert", ClientTransfert);

				Action<string> rackArrivantAction = OnSelectedRackArrivant;
				var eventOnSelectRackArrivant = EventCallback.Factory.Create(this, rackArrivantAction);
				builder.AddAttribute(7, "SelectRackArrivant", eventOnSelectRackArrivant);

				Action<string> rackPartantAction = OnSelectedRackPartant;
				var eventOnSelectRackPartant = EventCallback.Factory.Create(this, rackPartantAction);
				builder.AddAttribute(8, "SelectRackPartant", eventOnSelectRackPartant);

				builder.CloseComponent();
			};

			DisplayRenderFragment = CreateCompo();
		}


		private void CloseTransfert()
		{
			DisplayRenderFragment = null;
			StateChange.Invoke();

			TransfertRackValidation = new TransfertRackValidation();
			ClientTransfert = new CommandeView();
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

				StateChange.Invoke();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "HangarViewModel - OnValidTransfert");
				Notification.Notify(NotificationSeverity.Success, "Error", "Erreur sur le transfert");
			}
		}

		public async void OnSelectedRackPartant(string rackPartant)
		{

			if (!string.IsNullOrEmpty(rackPartant))
			{
				Rack rackSelected = RacksFull.FirstOrDefault(x => x.GisementPos == rackPartant);

				if (rackSelected != null)
				{
					TransfertRackValidation.IdRackPartant = rackSelected.IdRack;

					var temp = await SqlContext.GetCommandeByIdRack(rackSelected.IdRack);

					ClientTransfert.NomClient = temp.NomClient;
					ClientTransfert.DescriptionCmd = temp.DescriptionCmd;
					ClientTransfert.IdClient = temp.IdClient;
					ClientTransfert.IdCommande = temp.IdCommande;

					TransfertRackValidation.GisementRackPartant = rackSelected.GisementPos;
					StateChange.Invoke();
				}
				else
				{
					TransfertRackValidation.IdRackPartant = 0;
					TransfertRackValidation.GisementRackPartant = string.Empty;

					ClientTransfert = new CommandeView();
				}
			}
		}

		public void OnSelectedRackArrivant(string rackArrivant)
		{
			if (!string.IsNullOrEmpty(rackArrivant))
			{
				Rack rackSelected = Racks.FirstOrDefault(x => x.GisementPos == rackArrivant);
				if (rackSelected != null)
				{
					TransfertRackValidation.IdRackArrivant = rackSelected.IdRack;
					TransfertRackValidation.GisementRackArrivant = rackSelected.GisementPos;
				}
				else
				{
					TransfertRackValidation.IdRackArrivant = 0;
					TransfertRackValidation.GisementRackArrivant = string.Empty;
				}
			}
		}

		#endregion

		#region Intervertir palette

		public void OpenIntervertir()
		{
			RenderFragment CreateCompo() => builder =>
			{
				builder.OpenComponent(0, typeof(IntervertirPalettes));

				var eventOnValidSubmitIntervertir = EventCallback.Factory.Create(this, OnValidIntervertir);
				builder.AddAttribute(1, "OnValidSubmitInvertion", eventOnValidSubmitIntervertir);

				builder.AddAttribute(2, "InversionPaletteValidation", IntervertirValidation);
				builder.AddAttribute(3, "RackFull", RacksFull);

				var eventOnValidCloseIntervertir = EventCallback.Factory.Create(this, CloseIntervertir);
				builder.AddAttribute(4, "CloseInvertion", eventOnValidCloseIntervertir);

				Action<string> rackArrivantAction = OnSelectedRackArrivantIntervertir;
				var eventOnSelectRackArrivant = EventCallback.Factory.Create(this, rackArrivantAction);
				builder.AddAttribute(5, "SelectRackArrivant", eventOnSelectRackArrivant);

				Action<string> rackPartantAction = OnSelectedRackPartantIntervertir;
				var eventOnSelectRackPartant = EventCallback.Factory.Create(this, rackPartantAction);
				builder.AddAttribute(6, "SelectRackPartant", eventOnSelectRackPartant);

				builder.CloseComponent();
			};

			DisplayRenderFragment = CreateCompo();
		}

		private void OnSelectedRackPartantIntervertir(string rackPartant)
		{
			if (!string.IsNullOrEmpty(rackPartant))
			{
				Rack rackSelected = RacksFull.FirstOrDefault(x => x.GisementPos == rackPartant);

				if(rackSelected != null)
				{
					IntervertirValidation.IdRackPartant = rackSelected.IdRack;
					IntervertirValidation.IdCommandePartant = AllHangar.Find(x => x.IdRack == rackSelected.IdRack).IdCommande;
					IntervertirValidation.GisementRackPartant = rackSelected.GisementPos;
				}
				else
				{
					IntervertirValidation.IdRackPartant = 0;
					IntervertirValidation.GisementRackPartant = string.Empty;
				}
			}
		}

		private void OnSelectedRackArrivantIntervertir(string rackArrivant)
		{
			if (!string.IsNullOrEmpty(rackArrivant))
			{
				Rack rackSelected = RacksFull.FirstOrDefault(x => x.GisementPos == rackArrivant);
				if(rackSelected != null)
				{
					IntervertirValidation.IdRackArrivant = rackSelected.IdRack;
					IntervertirValidation.IdCommandeArrivant = AllHangar.Find(x => x.IdRack == rackSelected.IdRack).IdCommande;
					IntervertirValidation.GisementRackArrivant = rackSelected.GisementPos;
				}
				else
				{
					IntervertirValidation.IdRackArrivant = 0;
					IntervertirValidation.GisementRackArrivant = string.Empty;
				}
			}
		}

		private async void OnValidIntervertir()
		{
			try
			{
				await SqlContext.IntervertirRackTo(IntervertirValidation.IdRackPartant, 
													IntervertirValidation.IdRackArrivant, 
													IntervertirValidation.IdCommandePartant);

				await SqlContext.IntervertirRackTo(IntervertirValidation.IdRackArrivant,
													IntervertirValidation.IdRackPartant,
													IntervertirValidation.IdCommandeArrivant);

				// Recharger
				RacksFull = await SqlContext.GetRackFull();
				AllHangar = await SqlContext.GetHangar();

				await HangarGrid.Reload();

				Notification.Notify(NotificationSeverity.Success, "Inversion OK", "Inversion effectuée");

				// Remise à zéro
				IntervertirValidation = new InversionPaletteValidation();

				StateChange.Invoke();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "HangarViewModel - OnValidIntervertir");
				Notification.Notify(NotificationSeverity.Success, "Error", "Erreur sur l'inversion");
			}
		}

		private void CloseIntervertir()
		{
			DisplayRenderFragment = null;
			StateChange.Invoke();

			IntervertirValidation = new InversionPaletteValidation();
		}

		#endregion

		#endregion


		private void SelectedRackFull<T>(string selected, T validator) where T : BaseValidation
		{
			if (!string.IsNullOrEmpty(selected))
			{
				Rack rackSelected = RacksFull.FirstOrDefault(x => x.GisementPos == selected);

				if (rackSelected != null)
				{
					validator.IdRack = rackSelected.IdRack;
					validator.GisementRack = rackSelected.GisementPos;
				}
				else
				{
					validator.GisementRack = string.Empty;
					validator.IdRack = 0;
				}
			}
		}

		private void SelectedRackEmpty<T>(string selected, T validator) where T : BaseValidation
		{
			if (!string.IsNullOrEmpty(selected))
			{
				Rack rackSelected = Racks.FirstOrDefault(x => x.GisementPos == selected);

				if (rackSelected != null)
				{
					validator.IdRack = rackSelected.IdRack;
					validator.GisementRack = rackSelected.GisementPos;
				}
				else
				{
					validator.GisementRack = string.Empty;
					validator.IdRack = 0;
				}
			}
		}

	}
}
