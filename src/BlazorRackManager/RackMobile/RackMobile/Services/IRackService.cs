using RackCore;
using RackCore.EntityView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace RackMobile.Services
{
	public interface IRackService
	{
		/// <summary>
		/// Test de l'URL d'un serveur RackManager
		/// </summary>
		/// <param name="adresseServer"></param>
		/// <returns></returns>
		Task<bool> TestServerUrl(string adresseServer);
		
		void ChangeServerAddress(string addressServer);
		

		/// <summary>
		/// Connexion au serveur
		/// </summary>
		/// <param name="login"></param>
		/// <param name="motDePasse"></param>
		/// <returns></returns>
		Task<string> Connect(string login, string motDePasse);
		
		/// <summary>
		/// Récupère les racks vides.
		/// </summary>
		/// <returns></returns>
		Task<List<Rack>> GetRacksEmpty();

		/// <summary>
		/// Récupère les racks occupés
		/// </summary>
		/// <returns></returns>
		Task<List<Rack>> GetRacksOqp();

		/// <summary>
		/// Récupère les informations sur le rack (client, commande,..)
		/// </summary>
		/// <param name="idRack"></param>
		/// <returns></returns>
		Task<HangarView> GetInfoRack(int idRack);
	}
}
