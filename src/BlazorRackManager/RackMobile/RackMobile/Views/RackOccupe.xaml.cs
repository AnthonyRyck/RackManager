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
	}
}