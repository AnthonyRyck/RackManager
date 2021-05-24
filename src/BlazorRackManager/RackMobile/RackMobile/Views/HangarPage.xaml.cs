using RackMobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace RackMobile.Views
{
	public partial class HangarPage : ContentPage
	{
		public HangarPage()
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
			var choixSelected = e.SelectedItem as ItemHangar;

			switch (choixSelected.ChoixMenu)
			{
				case "RacksVide":
					await Navigation.PushAsync(new RackVidePage());
					break;

				case "RacksOqp":
					await Navigation.PushAsync(new RackOccupe());
					break;
				default:
					break;
			}
		}

	}
}