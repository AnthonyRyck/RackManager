﻿using AccessData;
using AccessData.Models;
using RackManager.ValidationModels;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public class RackViewModel : IRackViewModel
	{
		/// <see cref="IRackViewModel.AllRacks"/>
		public List<Rack> AllRacks { get; set; }

		/// <see cref="IClientViewModel.NouveauRack"/>
		public RackValidation NouveauRack { get; set; }

		/// <see cref="IClientViewModel.IsLoaded"/>
		public bool IsLoaded { get; set; }

		/// <see cref="IClientViewModel.DialogIsOpenNewClient"/>
		public bool DialogIsOpenNewRack { get; set; }

		/// <see cref="IClientViewModel.ClientGrid"/>
		public RadzenGrid<Rack> RackGrid { get; set; }

		private SqlContext SqlContext;
		private NotificationService NotificationService;

		public RackViewModel(SqlContext sqlContext, NotificationService notificationService)
		{
			IsLoaded = false;
			SqlContext = sqlContext;
			DialogIsOpenNewRack = false;
			NotificationService = notificationService;

			NouveauRack = new RackValidation();
			LoadRacks().GetAwaiter().GetResult();
		}

		#region Public methods

		/// <see cref="IClientViewModel.OpenNewClient"/>
		public void OpenNewRack()
		{
			DialogIsOpenNewRack = true;
		}

		/// <see cref="IClientViewModel.CloseNouveauClient"/>
		public void CloseNouveauRack()
		{
			DialogIsOpenNewRack = false;
			NouveauRack = new RackValidation();
		}

		/// <see cref="IClientViewModel.OnValidSubmit"/>
		public async void OnValidSubmit()
		{
			try
			{
				// Ajout dans la base de donnée.
				int idRack = await SqlContext.AddRack(NouveauRack.Gisement, NouveauRack.Position);

				Rack nouveauRack = new Rack()
				{
					IdRack = idRack,
					Gisement = NouveauRack.Gisement,
					PosRack = NouveauRack.Position
				};

				AllRacks.Add(nouveauRack);
				await RackGrid.Reload();

				string message = $"Nouveau rack : {nouveauRack.Gisement} ajouté";
				NotificationMessage messNotif = new NotificationMessage()
				{
					Summary = "Sauvegarde OK",
					Detail = message,
					Duration = 3000,
					Severity = NotificationSeverity.Success
				};
				NotificationService.Notify(messNotif);

				NouveauRack = new RackValidation();
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Charge tous les racks.
		/// </summary>
		/// <returns></returns>
		private async Task LoadRacks()
		{
			AllRacks = (await SqlContext.LoadRacks()).ToList();
			IsLoaded = true;
		}

		#endregion
	}
}
