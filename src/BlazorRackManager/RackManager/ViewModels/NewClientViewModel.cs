using AccessData;
using AccessData.Models;
using Microsoft.AspNetCore.Components;
using RackManager.ValidationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public class NewClientViewModel : INewClientViewModel
	{
		public ClientValidation NouveauClient { get; set; }

		private NavigationManager Navigation;
		private SqlContext SqlContext;

		public NewClientViewModel(NavigationManager navigation, SqlContext sqlContext)
		{
			NouveauClient = new ClientValidation();
			Navigation = navigation;
			SqlContext = sqlContext;
		}


		public async void OnValidSubmit()
		{
			try
			{
				// Ajout dans la base de donnée.
				await SqlContext.AddClient(NouveauClient.NomClient);

				// Retour à la page Setting
				ClosePage();
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		public void ClosePage()
		{
			Navigation.NavigateTo("settings");
		}
	}
}
