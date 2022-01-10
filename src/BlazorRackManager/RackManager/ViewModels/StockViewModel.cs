using AccessData;
using AccessData.Views;
using Microsoft.AspNetCore.Components;
using RackCore;
using RackCore.EntityView;
using RackManager.Composants;
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
	public class StockViewModel : IStockViewModel
	{
		public bool IsLoaded { get; set; }

		public List<StockView> AllStock { get; set; }

		public RenderFragment DisplayRenderFragment { get; set; }

		public RadzenGrid<StockView> StockGrid { get; set; }


		private SqlContext ContextSql;
		private NotificationService Notification;

		private EntreStockValidation StockValidation;
		private IEnumerable<Rack> Racks;
		private IEnumerable<ProduitView> Produits;
		private Action StateHasChange;

		public StockViewModel(SqlContext sqlContext, NotificationService notification)
		{
			ContextSql = sqlContext;
			Notification = notification;

			StockValidation = new EntreStockValidation();
		}


		public async Task LoadStocks()
		{
			try
			{
				AllStock = await ContextSql.GetStocks();
				Racks = await ContextSql.LoadRacks();
				Produits = await ContextSql.GetProduits();

				IsLoaded = true;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "StockViewModel - LoadStocks");
				Notification.Notify(NotificationSeverity.Error, "Erreur chargement", "Erreur sur le chargement desstocks.");
			}
		}

		public void SetStateHasChanged(Action stateHasChange)
		{
			StateHasChange = stateHasChange;
		}


		#region Nouvelle entré

		public void OpenNouvelleEntre()
		{
			RenderFragment CreateCompo() => builder =>
			{
				builder.OpenComponent(0, typeof(AjouterStock));
				builder.AddAttribute(1, "EntreStockValidation", StockValidation);
				builder.AddAttribute(2, "Racks", Racks);
				builder.AddAttribute(3, "Produits", Produits);

				// Ajout pour EventCallBack
				var eventTerminerEntre = EventCallback.Factory.Create(this, CloseEntre);
				builder.AddAttribute(4, "OnTerminerClick", eventTerminerEntre);

				Action<string> retourRack = OnSelectedRack;
				var eventOnSelectRack = EventCallback.Factory.Create(this, retourRack);
				builder.AddAttribute(6, "OnSelectedRack", eventOnSelectRack);

				Action<string> retourProduit = OnSelectedProduit;
				var eventOnSelecProduit = EventCallback.Factory.Create(this, retourProduit);
				builder.AddAttribute(7, "OnSelectedProduit", eventOnSelecProduit);

				var eventOnValidSubmitEntre = EventCallback.Factory.Create(this, OnValidSubmit);
				builder.AddAttribute(8, "OnValidSubmit", eventOnValidSubmitEntre);

				builder.CloseComponent();
			};

			DisplayRenderFragment = CreateCompo();
		}



		private async void OnValidSubmit()
		{
			try
			{
				if(StockValidation.IdRack == 0)
				{
					Notification.Notify(NotificationSeverity.Warning, "Attention", "Le Rack choisi n'est pas bon.");
					return;
				}

				if (string.IsNullOrEmpty(StockValidation.ReferenceProduit))
				{
					Notification.Notify(NotificationSeverity.Warning, "Attention", "La référence de produit choisie n'est pas bonne.");
					return;
				}

				Stock stock = StockValidation.ToStock();
				await ContextSql.AddNewStock(stock);

				Notification.Notify(NotificationSeverity.Success, "Sauvegarde OK", "Sauvegarde OK");

				// remise à zéro
				StockValidation = new EntreStockValidation();
				StateHasChange.Invoke();

				// Récupération de l'enregistrement
				StockView newStock = await ContextSql.GetStocks(stock.RackId, stock.ProduitId);

				Log.Information($"STOCK ENTREE - Ajout de {newStock.Quantite} de {newStock.ReferenceProduit} sur {newStock.GisementPos}");

				AllStock.Add(newStock);
				await StockGrid.Reload();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "StockViewModel - OnValidSubmit");
				Notification.Notify(NotificationSeverity.Success, "Error", "Erreur sur la sauvegarde");
			}
		}

		private void OnSelectedRack(string selected)
		{
			if (!string.IsNullOrEmpty(selected))
			{
				Rack rackSelected = Racks.FirstOrDefault(x => x.GisementPos == selected);

				if (rackSelected != null)
				{
					StockValidation.IdRack = rackSelected.IdRack;
					StockValidation.GisementRack = rackSelected.GisementPos;
				}
				else
				{
					StockValidation.GisementRack = string.Empty;
					StockValidation.IdRack = 0;
				}
			}
		}

		private void OnSelectedProduit(string produitSelected)
		{
			if (!string.IsNullOrEmpty(produitSelected))
			{
				ProduitView produitSelect = Produits.FirstOrDefault(x => x.IdReference == produitSelected);

				if(produitSelect != null)
				{
					StockValidation.ReferenceProduit = produitSelect.IdReference;
				}
				else
				{
					StockValidation.ReferenceProduit = string.Empty;
				}
			}
		}

		private void CloseEntre()
		{
			DisplayRenderFragment = null;
			StockValidation = new EntreStockValidation();
			StateHasChange.Invoke();
		}

		#endregion

		#region Event DataGrid


		public decimal? MaxQuantite { get; private set; }
		public decimal? MinQuantite { get; private set; }

		public bool RowOnUpdate { get; private set; } = false;

		public async void OnUpdateRow(StockView stock)
		{
			try
			{
				// Suppression du stock
				if (stock.Quantite == 0)
				{
					await ContextSql.DeleteStockLine(stock.IdRack, stock.ReferenceProduit);
					Log.Information($"STOCK - Sortie : {stock.ReferenceProduit} - quantité mis à ZERO de {stock.GisementPos}");
					AllStock = await ContextSql.GetStocks();
				}
				else
				{
					await ContextSql.UpdateStock(stock.IdRack, stock.ReferenceProduit, stock.Quantite);

					// Si ce n'est pas un ajout de stock
					if (EstAjoutStock)
					{
						var quantiteEntre = stock.Quantite - backup.Quantite;
						Log.Information($"STOCK - Ajout : {stock.ReferenceProduit} - quantité : {quantiteEntre} {stock.Unite} sur {stock.GisementPos}");
					}
					else
					{
						var quantiteSortie = backup.Quantite - stock.Quantite;
						await ContextSql.AddNewSortieStock(stock.ReferenceProduit, quantiteSortie, DateTime.Now);

						Log.Information($"STOCK - Sortie : {stock.ReferenceProduit} - quantité : {quantiteSortie} {stock.Unite} de {stock.GisementPos}");
					}
				}

				RowOnUpdate = false;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Erreur sur OnUpdateRow");
			}
		}

		private StockView backup;
		private bool EstAjoutStock;


		public void EditRow(StockView stock, bool isAdd)
		{
			backup = stock.DeepCopy();
			RowOnUpdate = true;
			EstAjoutStock = isAdd;

			if (isAdd)
			{
				MinQuantite = Convert.ToDecimal(stock.Quantite);
				MaxQuantite = decimal.MaxValue;
			}
			else
			{
				MinQuantite = 0;
				MaxQuantite = Convert.ToDecimal(stock.Quantite);
			}

			StockGrid.EditRow(stock);
		}

		public async void SaveRow(StockView stock)
		{
			await StockGrid.UpdateRow(stock);
			RowOnUpdate = false;
		}

		public void CancelEdit(StockView stock)
		{
			stock.Quantite = backup.Quantite;

			StockGrid.CancelEditRow(stock);
			RowOnUpdate = false;
		}

		#endregion
	}
}
