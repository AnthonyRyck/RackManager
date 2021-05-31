using AccessData;
using AccessData.Views;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using RackCore;
using RackManager.ValidationModels;
using Radzen;
using Radzen.Blazor;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public class ProduitViewModel : IProduitViewModel
	{
		public bool IsLoaded { get;	private set; }

		public bool CanOpenNewProduit { get; private set; }

		public bool HaveImage { get; private set; }

		public string ImageEnString { get; private set; }


		public List<ProduitView> Produits { get; set; }

		public IEnumerable<UniteMesure> Mesures { get; set; }

		public RadzenGrid<ProduitView> ProduitGrid { get; set; }

		public ProduitValidation NouveauProduit { get; set; }

		private SqlContext ContextSql;
		private NotificationService Notification;

		public ProduitViewModel(SqlContext sqlContext, NotificationService notificationService)
		{
			ContextSql = sqlContext;
			Notification = notificationService;

			NouveauProduit = new ProduitValidation();
			
			IsLoaded = false;
			HaveImage = false;
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
			NouveauProduit = new ProduitValidation();
		}

		public async Task OnValidSubmitProduit()
		{
			try
			{
				// Ajout dans la base de donnée.
				await ContextSql.AddProduit(NouveauProduit.ToProduit());

				var produitView = await ContextSql.GetProduits(NouveauProduit.Reference);
				Produits.Add(produitView);

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
			ImageEnString = string.Empty;
			HaveImage = false;
		}


		public void OnChangeMesure(ChangeEventArgs e)
		{
			try
			{
				if(e.Value.ToString() != "noid")
					NouveauProduit.IdMesure = Convert.ToInt32(e.Value);
				else
				{
					NouveauProduit.IdMesure = null;
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "OnChangeMesure - Erreur sur le changement d'unité");
				Notification.Notify(NotificationSeverity.Error, "Erreur", "Erreur sur le changement d'unité de mesure");
			}
		}


		public async Task UploadFiles(IMatFileUploadEntry[] files)
		{
			try
			{
				if (files.Count() == 1)
				{
					IMatFileUploadEntry fileMat = files.FirstOrDefault();

					if (fileMat.Type.Contains("image/jpeg")
						|| fileMat.Type.Contains("image/png"))
					{
						using (var streamTemp = new MemoryStream())
						{
							await fileMat.WriteToStreamAsync(streamTemp);
							NouveauProduit.ImgContent = streamTemp.ToArray();
						}

						ImageEnString = "data:image/png;base64," + Convert.ToBase64String(NouveauProduit.ImgContent);
						HaveImage = true;
					}
					else
					{
						Notification.Notify(NotificationSeverity.Warning, "Erreur", "Accepte format JPG ou PNG.");
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "UploadFiles - Erreur sur le changement de l'image");
				Notification.Notify(NotificationSeverity.Error, "Erreur", "Erreur sur le changement de l'image");
				HaveImage = false;
			}
		}

	}
}
