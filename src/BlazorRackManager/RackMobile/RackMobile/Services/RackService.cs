using Newtonsoft.Json;
using RackCore;
using RackMobile;
using RackMobile.Services;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;


namespace RackMobile.Services
{
	public class RackService : IRackService
	{
		private HttpClient Client;

		private string AdresseServeur;

		public bool IsServerAdressOk { get; set; }

		public RackService()
		{
			AdresseServeur = App.SettingManager.Setting.AddressServer;

			if (string.IsNullOrEmpty(AdresseServeur))
			{
				IsServerAdressOk = false;
				return;
			}

			// Pour ignorer les erreurs SSL
			var httpClientHandler = new HttpClientHandler();
			httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

			Client = new HttpClient(httpClientHandler)
			{
				BaseAddress = new Uri(AdresseServeur)
			};
			IsServerAdressOk = true;
		}
		
		public async Task<bool> TestServerUrl(string addresseServer)
		{
			bool testResult = false;

			try
			{
				var httpClientHandler = new HttpClientHandler();
				httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

				HttpClient clientTest = new HttpClient(httpClientHandler)
				{
					BaseAddress = new Uri(addresseServer)
				};

				HttpResponseMessage response = await clientTest.GetAsync("api/myconnect/testconnect/");

				if (response.IsSuccessStatusCode)
				{
					string content = await response.Content.ReadAsStringAsync();
					
					if (content == "Connexion OK")
						testResult = true;
				}
			}
			catch (Exception ex)
			{
				//Debug.WriteLine(@"\tERROR {0}", ex.Message);
			}

			return testResult;
		}

		public void ChangeServerAddress(string addressServer)
		{
			AdresseServeur = addressServer;

			// Pour ignorer les erreurs SSL
			var httpClientHandler = new HttpClientHandler();
			httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

			Client = new HttpClient(httpClientHandler)
			{
				BaseAddress = new Uri(addressServer)
			};

			IsServerAdressOk = true;
		}

		public async Task<string> Connect(string login, string motDePasse)
		{
			UserCredential user = new UserCredential()
			{
				Login = login,
				Password = motDePasse
			};

			using (var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"))
			{
				HttpResponseMessage result = Client.PostAsync("api/myconnect/authenticate/", content).Result;

				if (result.StatusCode == System.Net.HttpStatusCode.OK)
				{
					string jwt = await result.Content.ReadAsStringAsync();
					return jwt;
				}
				else
				{
					throw new Exception();
				}
			}
		}

		//public async Task<List<InformationMovie>> GetEmptyRack()
		//{
		//	List<InformationMovie> allMovies = new List<InformationMovie>();

		//	if (!IsServerAdressOk)
		//		return allMovies;

		//	try
		//	{
		//		HttpResponseMessage response = await Client.GetAsync("api/Movies/allmovies");
		//		if (response.IsSuccessStatusCode)
		//		{
		//			string content = await response.Content.ReadAsStringAsync();
		//			allMovies = JsonConvert.DeserializeObject<List<InformationMovie>>(content);
		//		}
		//	}
		//	catch (Exception)
		//	{
		//		throw;
		//	}

		//	return allMovies;
		//}

	}
}
