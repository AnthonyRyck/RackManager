using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RackMobile.Views.Stock
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RacksStockPage : ContentPage
	{
		public RacksStockPage()
		{
			InitializeComponent();
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			try
			{
				await ViewModel.Init();
			}
			catch (UnauthorizedAccessException)
			{
				await DisplayAlert("Erreur", "Vous n'êtes pas autorisé", "OK");
			}
			catch (Exception)
			{
				await DisplayAlert("Erreur", "Erreur sur la récupération des racks occupés", "OK");
			}
		}

		private void Recherche_TextChanged(object sender, TextChangedEventArgs e)
		{
			string gisement = e.NewTextValue.ToUpper();
			ViewModel.Filtrer(gisement);
		}
	}
}