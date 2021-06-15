using AccessData.Views;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using RackCore;
using RackManager.ValidationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public interface IProduitViewModel
	{
		/// <summary>
		/// Indique si le produit est chargé en mémoire.
		/// </summary>
		bool IsLoaded { get; }

		/// <summary>
		/// Nom du produit
		/// </summary>
		string ProduitName { get; }

		/// <summary>
		/// C'est le produit à afficher
		/// </summary>
		ProduitView Produit { get; }

		/// <summary>
		/// Indicateur qu'il faut modifier le produit.
		/// </summary>
		bool IsModified { get; }

		/// <summary>
		/// Objet de validation d'un produit.
		/// </summary>
		ProduitValidation UpdateProduit { get; }

		/// <summary>
		/// Indicateur s'il y a une image de mise à jour.
		/// </summary>
		bool HaveUpdateImage { get; }

		/// <summary>
		/// Est la mise à jour de l'image en string base 64
		/// </summary>
		string ImageUpdateEnString { get; }

		/// <summary>
		/// Liste des unités de mesure
		/// </summary>
		IEnumerable<UniteMesure> Mesures { get; }

		/// <summary>
		/// Charge le produit donnée en paramètre
		/// </summary>
		/// <param name="referenceProduit"></param>
		/// <returns></returns>
		Task LoadProduit(string referenceProduit);

		/// <summary>
		/// Permet de modifier le produit.
		/// </summary>
		void ModifierProduit();



		Task OnValidSubmitProduit();

		void CloseUpdateProduit();

		void OnChangeMesure(ChangeEventArgs e);


		Task UploadFile(IMatFileUploadEntry[] files);
	}
}
