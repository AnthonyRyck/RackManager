using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RackMobile.ViewModels;

namespace RackMobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingPage : ContentPage
	{
		private SettingViewModel settingViewModel;

		public SettingPage()
		{
			InitializeComponent();
			settingViewModel = (SettingViewModel)BindingContext;
		}


		protected override void OnAppearing()
		{
			base.OnAppearing();
			settingViewModel.LoadSetting();
		}


		private async void OnTestClicked(object sender, EventArgs e)
		{
			try
			{
				await settingViewModel.TestServeur();
			}
			catch (Exception)
			{
				await DisplayAlert("Erreur", "Erreur sur le test du serveur", "OK");
			}
		}

		private async void OnSaveClicked(object sender, EventArgs e)
		{
			try
			{
				settingViewModel.SaveServeur();
			}
			catch (Exception)
			{
				await DisplayAlert("Erreur", "Erreur sur la sauvegarde du serveur", "OK");
			}
		}

		private async void OnConnexionCicked(object sender, EventArgs e)
		{
			try
			{
				settingViewModel.ConnexionServeur();
			}
			catch (Exception)
			{
				await DisplayAlert("Erreur", "Erreur sur la sauvegarde du serveur", "OK");
			}
		}
		
	}
}