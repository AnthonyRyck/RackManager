using System;
using System.Threading.Tasks;
using RackMobile.Models;

namespace RackMobile.ViewModels
{
	public class SettingViewModel : BaseViewModel
	{
		#region Properties

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
		/// Indicateur que le test serveur est en cours.
		/// </summary>
		public bool TestServerIsRun
		{
			get { return _testServerIsRun; }
			set
			{
				_testServerIsRun = value;
				OnNotifyPropertyChanged();
			}
		}
		private bool _testServerIsRun;

		/// <summary>
		/// Indicateur que le test de connexion est en cours.
		/// </summary>
		public bool TestConnexionIsRun
		{
			get { return _testConnexionIsRun; }
			set
			{
				_testConnexionIsRun = value;
				OnNotifyPropertyChanged();
			}
		}
		private bool _testConnexionIsRun;

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

		/// <summary>
		/// Indicateur s'il y a un serveur
		/// </summary>
		public bool HaveServerAddress
		{
			get { return _haveServerAddress; }
			set
			{
				_haveServerAddress = value;
				OnNotifyPropertyChanged();
			}
		}
		private bool _haveServerAddress;

		/// <summary>
		/// Login de connexion
		/// </summary>
		public string Login
		{
			get { return _login; }
			set
			{
				_login = value;
				OnNotifyPropertyChanged();
			}
		}
		private string _login;

		/// <summary>
		/// Mot de passe de connexion
		/// </summary>
		public string MotDePasse
		{
			get { return _motDePasse; }
			set
			{
				_motDePasse = value;
				OnNotifyPropertyChanged();
			}
		}
		private string _motDePasse;

		/// <summary>
		/// Indique que la sauvegarde est OK pour le
		/// login et MDP
		/// </summary>
		public bool IsSaveOkLogin
		{
			get { return _isSaveOkLogin; }
			set
			{
				_isSaveOkLogin = value;
				OnNotifyPropertyChanged();
			}
		}
		private bool _isSaveOkLogin;

		public string ResultatLogin
		{
			get { return _resultatLogin; }
			set
			{
				_resultatLogin = value;
				OnNotifyPropertyChanged();
			}
		}
		private string _resultatLogin;

		public Setting Setting { get; set; }

		#endregion

		internal void LoadSetting()
		{
			App.SettingManager.LoadSetting();
			var setting = App.SettingManager.Setting;

			AdresseServer = setting.AddressServer;

			if (!string.IsNullOrEmpty(AdresseServer))
			{
				HaveServerAddress = true;
			}
		}


		public async Task TestServeur()
		{
			TestServerIsRun = true;

			if (!HaveSlash(AdresseServer))
				AdresseServer += "/";

			IsTestOk = await RackService.TestServerUrl(AdresseServer);

			if(IsTestOk)
			{
				ResultTest = "Connexion serveur OK.";
			}
			else
			{
				ResultTest = "Pas de connexion au serveur demandé réussie.";
			}

			TestServerIsRun = false;
		}


		public void SaveServeur()
		{
			App.SettingManager.SaveServeur(AdresseServer);
			RackService.ChangeServerAddress(AdresseServer);
			IsSaveOk = true;
			HaveServerAddress = true;
		}


		public async Task ConnexionServeur()
		{
			TestConnexionIsRun = true;
			try
			{
				string tokenJwt = await RackService.Connect(Login, MotDePasse);
				App.SettingManager.SaveToken(tokenJwt);
				IsSaveOkLogin = true;
				ResultatLogin = "Login OK";
			}
			catch (Exception)
			{
				IsSaveOkLogin = false;
				ResultatLogin = "Erreur de connexion avec le login";
			}
			TestConnexionIsRun = false;
		}

		#region Private methods

		private bool HaveSlash(string adresseServer)
		{
			return adresseServer[adresseServer.Length - 1] == '/';
		}

		#endregion

	}
}
