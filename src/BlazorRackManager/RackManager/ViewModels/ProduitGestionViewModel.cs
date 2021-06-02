using AccessData;
using AccessData.Views;
using MatBlazor;
using Microsoft.AspNetCore.Components;
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
using System.Drawing;

namespace RackManager.ViewModels
{
	public class ProduitGestionViewModel : IGestionProduitViewModel
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
		private NavigationManager NavManager;

		public ProduitGestionViewModel(SqlContext sqlContext, NotificationService notificationService, NavigationManager navigationManager)
		{
			ContextSql = sqlContext;
			Notification = notificationService;
			NavManager = navigationManager;

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

							// Convertion de l'image en thumbnail
							using (Image newImgThumbnail = GetReducedImage(200, 200, streamTemp))
							{
								using (MemoryStream thumbStream = new MemoryStream())
								{
									newImgThumbnail.Save(thumbStream, System.Drawing.Imaging.ImageFormat.Png);
									NouveauProduit.ImgContent = thumbStream.ToArray();

									ImageEnString = "data:image/png;base64," + Convert.ToBase64String(NouveauProduit.ImgContent);
								}	
							}
						}

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


		public void OpenProduitPage(string idReference)
		{
			NavManager.NavigateTo($"/produit/{idReference}");
		}


		
		private Image GetReducedImage(int width, int height, Stream resourceImage)
		{
			try
			{
				var image = Image.FromStream(resourceImage);
				var thumb = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);

				return thumb;
			}
			catch (Exception e)
			{
				return null;
			}
		}
	}
}
