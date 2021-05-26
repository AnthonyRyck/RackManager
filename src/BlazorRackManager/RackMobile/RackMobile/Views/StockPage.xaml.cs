using RackMobile.Models;
using RackMobile.Views.Stock;
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
	public partial class StockPage : ContentPage
	{
		public StockPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			ViewModel.Init();
			base.OnAppearing();
		}

		private async void OnChoixSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var choixSelected = e.SelectedItem as ItemChoice;

			switch (choixSelected.ChoixMenu)
			{
				case "RefProduit":
					await Navigation.PushAsync(new ProduitStockPage());
					break;

				case "RacksStock":
					await Navigation.PushAsync(new RacksStockPage());
					break;
				default:
					break;
			}
		}
	}
}