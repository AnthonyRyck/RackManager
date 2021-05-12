using RackCore;
using RackMobile.ViewModels;
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
	public partial class RackOqpDetailPage : ContentPage
	{
		private RackDetailViewModel viewModel;

		public RackOqpDetailPage(Rack rack)
		{
			InitializeComponent();
			viewModel = new RackDetailViewModel(rack);
			viewModel.Title = "Info sur le rack";
			BindingContext = viewModel;
		}



		protected async override void OnAppearing()
		{
			base.OnAppearing();

			try
			{
				await viewModel.LoadInfoRack();
			}
			catch (UnauthorizedAccessException)
			{
				await DisplayAlert("Erreur", "Vous n'êtes pas autorisé", "OK");
			}
			catch (Exception ex)
			{
				await DisplayAlert("Erreur", "Erreur sur la récupération des informations sur racks", "OK");
			}
		}
	}
}