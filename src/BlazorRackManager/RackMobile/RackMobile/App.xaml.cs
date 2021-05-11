using RackMobile.Services;
using RackMobile.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RackMobile
{
	public partial class App : Application
	{
		public static SettingManager SettingManager { get; set; }


		public App()
		{
			InitializeComponent();

			SettingManager = new SettingManager();
			SettingManager.LoadSetting();

			DependencyService.Register<IRackService, RackService>();
			MainPage = new AppShell();
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}
