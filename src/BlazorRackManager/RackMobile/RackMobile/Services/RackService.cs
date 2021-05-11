using Newtonsoft.Json;
using RackCore;
using RackMobile;
using RackMobile.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;


namespace RackMobile.Services
{
	public class RackService : IRackService
	{
		private HttpClient ClientHttp;

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

			ClientHttp = new HttpClient(httpClientHandler)
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

			ClientHttp = new HttpClient(httpClientHandler)
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
				HttpResponseMessage result = ClientHttp.PostAsync("api/myconnect/authenticate/", content).Result;

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


		public async Task<List<Rack>> GetRacksEmpty()
		{
			List<Rack> racksEmpty = new List<Rack>();

			if (!IsServerAdressOk)
				return racksEmpty;

			racksEmpty = await Get<List<Rack>>("api/Hangar/rackempty");

			return racksEmpty;
		}

		public async Task<List<Rack>> GetRacksOqp()
		{
			List<Rack> racksEmpty = new List<Rack>();

			if (!IsServerAdressOk)
				return racksEmpty;

			racksEmpty = await Get<List<Rack>>("api/Hangar/rackoqp");

			return racksEmpty;
		}


		/// <summary>
		/// Pour toutes les méthodes Http GET
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="urlApi"></param>
		/// <returns></returns>
		private async Task<T> Get<T>(string urlApi)
		{
			T result = default;

			try
			{
				// Pour avoir la derniere version du token.
				if(ClientHttp.DefaultRequestHeaders.Contains("Authorization"))
				{
					ClientHttp.DefaultRequestHeaders.Remove("Authorization");
				}

				ClientHttp.DefaultRequestHeaders.Add("Authorization", "Bearer " + App.SettingManager.Setting.TokenJwt);

				HttpResponseMessage response = await ClientHttp.GetAsync(urlApi);
				if (response.IsSuccessStatusCode)
				{
					string content = await response.Content.ReadAsStringAsync();
					result = JsonConvert.DeserializeObject<T>(content);
				}

				if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
				{
					throw new UnauthorizedAccessException();
				}
			}
			catch (Exception)
			{
				throw;
			}

			return result;
		}

	}
}
