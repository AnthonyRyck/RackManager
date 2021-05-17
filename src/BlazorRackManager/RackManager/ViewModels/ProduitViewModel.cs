using AccessData;
using AccessData.Views;
using Microsoft.AspNetCore.Components;
using RackCore;
using RackManager.ValidationModels;
using Radzen;
using Radzen.Blazor;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public class ProduitViewModel : IProduitViewModel
	{
		public bool IsLoaded { get;	private set; }

		public bool CanOpenNewProduit { get; private set; }


		public List<ProduitView> Produits { get; set; }

		public IEnumerable<UniteMesure> Mesures { get; set; }

		public RadzenGrid<ProduitView> ProduitGrid { get; set; }

		public ProduitValidation NouveauProduit { get; set; }

		private SqlContext ContextSql;
		private NotificationService Notification;

		public ProduitViewModel(SqlContext sqlContext, NotificationService notificationService)
		{
			ContextSql = sqlContext;

			IsLoaded = false;
		}



		public async Task LoadProduits()
		{
			try
			{
				Produits = await ContextSql.GetProduits();
				Mesures = await ContextSql.GetUniteMesure();

				IsLoaded = true;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "ProduitViewModel - LoadProduits");
				Notification.Notify(NotificationSeverity.Error, "Erreur", "Erreur sur le chargement");
			}
		}


		public void OpenNewProduit()
		{
			CanOpenNewProduit = true;
		}

		public void CloseNewProduit()
		{
			CanOpenNewProduit = false;

		}

		public async Task OnValidSubmitProduit()
		{
			try
			{
				// Ajout dans la base de donnée.
				await ContextSql.AddProduit(NouveauProduit.ToProduit());

				var produitView = await ContextSql.GetProduits(NouveauProduit.Reference);
				Produits.Add(produitView);
				await ProduitGrid.Reload();

				string message = $"Nouveau produit - ref:{produitView.IdReference} - {produitView.Nom} ajouté";
				NotificationMessage messNotif = new NotificationMessage()
				{
					Summary = "Sauvegarde OK",
					Detail = message,
					Duration = 3000,
					Severity = NotificationSeverity.Success
				};
				Notification.Notify(messNotif);

				Log.Information("PRODUIT - " + message);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "ProduitViewModel - OnValidSubmitProduit");
				Notification.Notify(NotificationSeverity.Error, "Erreur", "Erreur sur la sauvegarde");
			}

			NouveauProduit = new ProduitValidation();
		}



		public void OnChangeMesure(ChangeEventArgs e)
		{

		}
	}
}
