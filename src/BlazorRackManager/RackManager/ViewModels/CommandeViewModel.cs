using AccessData;
using AccessData.Models;
using AccessData.Views;
using RackManager.ValidationModels;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public class CommandeViewModel : ICommandeViewModel
	{
		/// <see cref="ICommandeViewModel.AllCommandes"/>
		public List<CommandeView> AllCommandes { get; set; }

		/// <see cref="ICommandeViewModel.AllClients"/>
		public IEnumerable<Client> AllClients { get; set; }

		/// <see cref="ICommandeViewModel.IsLoaded"/>
		public bool IsLoaded { get; set; }

		/// <see cref="ICommandeViewModel.DialogIsOpenNewCommande"/>
		public bool DialogIsOpenNewCommande { get; set; }

		/// <see cref="ICommandeViewModel.NouvelleCommande"/>
		public CommandeValidation NouvelleCommande { get; set; }

		/// <see cref="ICommandeViewModel.CommandeGrid"/>
		public RadzenGrid<CommandeView> CommandeGrid { get; set; }

		private SqlContext SqlContext;
		private NotificationService NotificationService;

		public CommandeViewModel(SqlContext sqlContext, NotificationService notification)
		{
			SqlContext = sqlContext;
			NotificationService = notification;
			IsLoaded = false;
			DialogIsOpenNewCommande = false;

			NouvelleCommande = new CommandeValidation();

			LoadAllCommandes().GetAwaiter().GetResult();
		}


		#region Public methods

		/// <see cref="ICommandeViewModel.CloseNouvelleCommande"/>
		public void CloseNouvelleCommande()
		{
			DialogIsOpenNewCommande = false;
			NouvelleCommande = new CommandeValidation();
		}

		/// <see cref="ICommandeViewModel.OnValidSubmit"/>
		public async void OnValidSubmit()
		{
			try
			{
				SuiviCommande cmd = NouvelleCommande.ToSuiviCommande();
				await SqlContext.AddCommande(cmd);

				CommandeView commande = new CommandeView()
				{
					IdClient = cmd.ClientId,
					NomClient = NouvelleCommande.NomClient,
					IdCommande = cmd.IdCommande,
					DescriptionCmd = cmd.DescriptionCmd
				};

				AllCommandes.Add(commande);
				await CommandeGrid.Reload();

				string message = $"Commande : {commande.IdCommande}" + Environment.NewLine
									+ $"Client : {commande.NomClient}";

				NouvelleCommande = new CommandeValidation();

				NotificationMessage messNotif = new NotificationMessage()
				{
					Summary = "Sauvegarde OK",
					Detail = message,
					Duration = 3000,
					Severity = NotificationSeverity.Success
				};
				NotificationService.Notify(messNotif);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		/// <see cref="ICommandeViewModel.OpenNewCommande"/>
		public void OpenNewCommande()
		{
			DialogIsOpenNewCommande = true;
		}

		/// <see cref="ICommandeViewModel.OnSelectClient"/>
		public void OnSelectClient(object client)
		{
			Client clientSelected = client as Client;

			if (clientSelected != null)
			{
				NouvelleCommande.IdClient = clientSelected.IdClient;
				NouvelleCommande.NomClient = clientSelected.NomClient;
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Charge tous les commandes.
		/// </summary>
		/// <returns></returns>
		private async Task LoadAllCommandes()
		{
			AllCommandes = (await SqlContext.LoadCommandes()).ToList();
			AllClients = (await SqlContext.LoadClients());
			IsLoaded = true;
		}

		#endregion
	}
}
