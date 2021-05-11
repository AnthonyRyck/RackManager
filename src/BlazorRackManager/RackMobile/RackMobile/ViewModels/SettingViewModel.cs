using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using RackMobile.Models;
using RackMobile.Services;

namespace RackMobile.ViewModels
{
	public class SettingViewModel : BaseViewModel
	{
		/// <summary>
		/// Affiche le résultat du test.
		/// </summary>
		public string ResultTest
		{
			get { return _resultTest; }
			set
			{
				_resultTest = value;
				OnNotifyPropertyChanged();
			}
		}
		private string _resultTest;

		/// <summary>
		/// Indique si le test est Ok.
		/// </summary>
		public bool IsTestOk
		{
			get { return _isTestOk; }
			set
			{
				_isTestOk = value;
				OnNotifyPropertyChanged();
			}
		}
		private bool _isTestOk;

		/// <summary>
		/// Indique que la sauvegarde est OK
		/// </summary>
		public bool IsSaveOk
		{
			get { return _isSaveOk; }
			set
			{
				_isSaveOk = value;
				OnNotifyPropertyChanged();
			}
		}
		private bool _isSaveOk;


		/// <summary>
		/// Adresse du serveur à tester.
		/// </summary>
		public string AdresseServer
		{
			get { return _adresseServer; }
			set
			{
				_adresseServer = value;
				OnNotifyPropertyChanged();
			}
		}
		private string _adresseServer;


		private IRackService ZeusService => DependencyService.Get<IRackService>();

		public Setting Setting { get; set; }


		internal void LoadSetting()
		{
			App.SettingManager.LoadSetting();
			var setting = App.SettingManager.Setting;

			AdresseServer = setting.AddressServer;
		}


		public async Task TestServeur()
		{
			if (!HaveSlash(AdresseServer))
				AdresseServer += "/";

			IsTestOk = await ZeusService.TestServerUrl(AdresseServer);

			if(IsTestOk)
			{
				ResultTest = "Connexion serveur OK.";
			}
			else
			{
				ResultTest = "Pas de connexion au serveur demandé réussie.";
			}
		}


		public void SaveServeur()
		{
			App.SettingManager.SaveServeur(AdresseServer);
			IsSaveOk = true;
		}


		private bool HaveSlash(string adresseServer)
		{
			return adresseServer[adresseServer.Length - 1] == '/';
		}
	}
}
