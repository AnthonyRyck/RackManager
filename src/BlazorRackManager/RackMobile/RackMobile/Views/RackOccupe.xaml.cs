using RackCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RackMobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RackOccupe : ContentPage
	{
		public RackOccupe()
		{
			InitializeComponent();
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			try
			{
				await ViewModel.LoadRacksFull();
			}
			catch (UnauthorizedAccessException)
			{
				await DisplayAlert("Erreur", "Vous n'êtes pas autorisé", "OK");
			}
			catch (Exception ex)
			{
				await DisplayAlert("Erreur", "Erreur sur la récupération des racks occupés", "OK");
			}
		}

		private void Recherche_TextChanged(object sender, TextChangedEventArgs e)
		{
			string gisement = e.NewTextValue.ToUpper();
			ViewModel.Rechercher(gisement);
		}

		private async void OnRackSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var rackSelected = e.SelectedItem as Rack;

			await Navigation.PushAsync(new RackOqpDetailPage(rackSelected));
		}
	}
}