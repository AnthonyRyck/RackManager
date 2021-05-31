using AccessData.Views;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using RackCore;
using RackManager.ValidationModels;
using Radzen.Blazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public interface IProduitViewModel
	{
		bool IsLoaded { get; }
		bool CanOpenNewProduit { get; }

		/// <summary>
		/// Utilisateur à mis une image.
		/// </summary>
		bool HaveImage { get; }

		/// <summary>
		/// Image du produit en string base 64
		/// </summary>
		string ImageEnString { get; }

		/// <summary>
		/// Liste de tous les produits
		/// </summary>
		List<ProduitView> Produits { get; }

		/// <summary>
		/// Liste des unités de mesure
		/// </summary>
		IEnumerable<UniteMesure> Mesures { get; set; }

		/// <summary>
		/// Référence au tableau Radzen
		/// </summary>
		RadzenGrid<ProduitView> ProduitGrid { get; set; }

		/// <summary>
		/// Nouveau produit avec validation
		/// </summary>
		ProduitValidation NouveauProduit { get; set; }


		/// <summary>
		/// Charge tous les produits
		/// </summary>
		/// <returns></returns>
		Task LoadProduits();

		/// <summary>
		/// Ouvre la boite de dialogue pour ajouter
		/// un nouveau produit.
		/// </summary>
		void OpenNewProduit();

		/// <summary>
		/// Ferme le panel.
		/// </summary>
		void CloseNewProduit();

		/// <summary>
		/// Méthode pour la validation d'un nouveau
		/// produit.
		/// </summary>
		/// <returns></returns>
		Task OnValidSubmitProduit();

		/// <summary>
		/// Pour recevoir le fichier envoyé pour l'image d'un produit.
		/// </summary>
		/// <param name="files"></param>
		/// <returns></returns>
		Task UploadFiles(IMatFileUploadEntry[] files);

		/// <summary>
		/// Pour changer la "mesure" du produit.
		/// </summary>
		/// <param name="e"></param>
		void OnChangeMesure(ChangeEventArgs e);
	}
}
