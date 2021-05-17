using AccessData.Views;
using Microsoft.AspNetCore.Components;
using RackCore;
using RackManager.ValidationModels;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public interface IProduitViewModel
	{
		bool IsLoaded { get; }
		bool CanOpenNewProduit { get; }

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



		void OnChangeMesure(ChangeEventArgs e);
	}
}
