using AccessData;
using AccessData.Models;
using Microsoft.AspNetCore.Components;
using RackManager.Composants;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public class ClientViewModel : IClientViewModel
	{
		/// <see cref="IClientViewModel.AllClients"/>
		public List<Client> AllClients { get; set; }

		/// <see cref="IClientViewModel.IsLoaded"/>
		public bool IsLoaded { get; set; }

		/// <see cref="IClientViewModel.DialogIsOpenNewClient"/>
		public bool DialogIsOpenNewClient { get; set; }

		private SqlContext SqlContext;
		private NavigationManager NavigationManager;

		

		public ClientViewModel(SqlContext sqlContext, NavigationManager navigationManager)
		{
			IsLoaded = false;
			SqlContext = sqlContext;
			NavigationManager = navigationManager;

			LoadClients().GetAwaiter().GetResult();
		}

		#region Public methods

		/// <see cref="IClientViewModel.OpenPageNewClient"/>
		public void OpenPageNewClient()
		{
			NavigationManager.NavigateTo("nouveauclient");
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Charge tous les clients.
		/// </summary>
		/// <returns></returns>
		private async Task LoadClients()
		{
			AllClients = (await SqlContext.LoadClients()).ToList();
			IsLoaded = true;
		}

		#endregion



	}
}
