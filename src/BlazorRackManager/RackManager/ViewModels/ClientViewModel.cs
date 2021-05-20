using AccessData;
using RackCore;
using Microsoft.AspNetCore.Components;
using RackManager.Composants;
using RackManager.ValidationModels;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace RackManager.ViewModels
{
	public class ClientViewModel : IClientViewModel
	{
		/// <see cref="IClientViewModel.AllClients"/>
		public List<Client> AllClients { get; set; }

		/// <see cref="IClientViewModel.NouveauClient"/>
		public ClientValidation NouveauClient { get; set; }

		/// <see cref="IClientViewModel.IsLoaded"/>
		public bool IsLoaded { get; set; }

		/// <see cref="IClientViewModel.DialogIsOpenNewClient"/>
		public bool DialogIsOpenNewClient { get; set; }

		/// <see cref="IClientViewModel.ClientGrid"/>
		public RadzenGrid<Client> ClientGrid { get; set; }

		private SqlContext SqlContext;
		private NotificationService NotificationService;

		public ClientViewModel(SqlContext sqlContext, NotificationService notificationService)
		{
			IsLoaded = false;
			SqlContext = sqlContext;
			DialogIsOpenNewClient = false;
			NotificationService = notificationService;

			NouveauClient = new ClientValidation();
		}

		#region Public methods

		/// <see cref="IClientViewModel.OpenNewClient"/>
		public void OpenNewClient()
		{
			DialogIsOpenNewClient = true;
		}

		/// <see cref="IClientViewModel.CloseNouveauClient"/>
		public void CloseNouveauClient()
		{
			DialogIsOpenNewClient = false;
			NouveauClient = new ClientValidation();
		}

		/// <see cref="IClientViewModel.OnValidSubmit"/>
		public async void OnValidSubmit()
		{
			try
			{
				// Si contient déjà le même nom de client.
				if (AllClients.Any(x => x.NomClient == NouveauClient.NomClient))
				{
					string msgWarn = $"Aucun ajout : {NouveauClient.NomClient} existe déjà";
					NotificationMessage messWarn = new NotificationMessage()
					{
						Summary = "Attention",
						Detail = msgWarn,
						Duration = 3000,
						Severity = NotificationSeverity.Warning
					};
					NotificationService.Notify(messWarn);

					return;
				}

				// Ajout dans la base de donnée.
				int idClient = await SqlContext.AddClient(NouveauClient.NomClient);

				Client nouveauClient = new Client()
				{
					IdClient = idClient,
					NomClient = NouveauClient.NomClient
				};
				
				AllClients.Add(nouveauClient);
				await ClientGrid.Reload();

				string message = $"Nouveau client : {nouveauClient.NomClient} ajouté";
				NotificationMessage messNotif = new NotificationMessage()
				{
					Summary = "Sauvegarde OK",
					Detail = message,
					Duration = 3000,
					Severity = NotificationSeverity.Success
				};
				NotificationService.Notify(messNotif);

				Log.Information("CLIENT - " + message);

				NouveauClient = new ClientValidation();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "ClientViewModel - OnValidSubmit");
			}
		}

		/// <summary>
		/// Charge tous les clients.
		/// </summary>
		/// <returns></returns>
		public async Task LoadClients()
		{
			IsLoaded = false;
			AllClients = (await SqlContext.LoadClients()).ToList();
			IsLoaded = true;
		}

		#endregion

	}
}
