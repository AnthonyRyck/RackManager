using AccessData;
using AccessData.Views;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using RackCore;
using RackManager.ValidationModels;
using Radzen;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public class ProduitViewModel : IProduitViewModel
	{

		/// <see cref="IProduitViewModel.IsLoaded"/>
		public bool IsLoaded { get; private set; }

		/// <see cref="IProduitViewModel.ProduitName"/>
		public string ProduitName { get; private set; }

		/// <see cref="IProduitViewModel.Produit"/>
		public ProduitView Produit { get; private set;  }

		public ProduitValidation UpdateProduit { get; set; }

		/// <see cref="IProduitViewModel.HaveUpdateImage"/>
		public bool HaveUpdateImage { get; private set; }

		/// <see cref="IProduitViewModel.ImageUpdateEnString"/>
		public string ImageUpdateEnString { get; private set; }

		/// <see cref="IProduitViewModel.IsModified"/>
		public bool IsModified { get; private set; }

		/// <see cref="IProduitViewModel.Mesures"/>
		public IEnumerable<UniteMesure> Mesures { get; private set; }

		private SqlContext ContextSql;
		private NotificationService Notification;

		public ProduitViewModel(SqlContext sqlContext, NotificationService notificationService)
		{
			ContextSql = sqlContext;
			Notification = notificationService;

			UpdateProduit = new ProduitValidation();
		}

		/// <see cref="IProduitViewModel.LoadProduit(string)"/>
		public async Task LoadProduit(string referenceProduit)
		{
			try
			{
				Produit = await ContextSql.GetProduits(referenceProduit);
				Mesures = await ContextSql.GetUniteMesure();
				LoadValidation();
			}
			catch (Exception ex)
			{
				string message = $"Erreur sur LoadProduit pour la référence {referenceProduit}";
				Log.Error(ex, message);
				Notification.Notify(NotificationSeverity.Error, "Erreur", message);
			}
		}

		private void LoadValidation()
		{
			UpdateProduit.Reference = Produit.IdReference;
			UpdateProduit.ImgContent = Produit.ImageProduit;
			UpdateProduit.IdMesure = Produit.IdMesure;
			UpdateProduit.NomProduit = Produit.Nom;
		}

		/// <see cref="IProduitViewModel.ModifierProduit"/>
		public void ModifierProduit()
		{
			IsModified = true;
		}

		public async Task OnValidSubmitProduit()
		{
			try
			{
				// Ajout dans la base de donnée.
				await ContextSql.UpdateProduit(UpdateProduit.Reference, UpdateProduit.ToProduit());
				Produit = await ContextSql.GetProduits(UpdateProduit.Reference);
				LoadValidation();

				string message = $"Mise à jour des informations du produit {Produit.IdReference}";
				NotificationMessage messNotif = new NotificationMessage()
				{
					Summary = "Mise à jour OK",
					Detail = message,
					Duration = 3000,
					Severity = NotificationSeverity.Success
				};
				Notification.Notify(messNotif);

				Log.Information("UPDATE PRODUIT - " + message);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "ProduitViewModel - OnValidSubmitProduit");
				Notification.Notify(NotificationSeverity.Error, "Erreur", "Erreur sur la sauvegarde");
			}

			ImageUpdateEnString = string.Empty;
			HaveUpdateImage = false;
			
			IsModified = false;
		}


		public void CloseUpdateProduit()
		{
			IsModified = false;
			LoadValidation();
		}

		public void OnChangeMesure(ChangeEventArgs e)
		{
			try
			{
				if (e.Value.ToString() != "noid")
					UpdateProduit.IdMesure = Convert.ToInt32(e.Value);
				else
				{
					UpdateProduit.IdMesure = null;
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "OnChangeMesure - Erreur sur le changement d'unité");
				Notification.Notify(NotificationSeverity.Error, "Erreur", "Erreur sur le changement d'unité de mesure");
			}
		}

		public async Task UploadFile(IMatFileUploadEntry[] files)
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
									UpdateProduit.ImgContent = thumbStream.ToArray();

									ImageUpdateEnString = "data:image/png;base64," + Convert.ToBase64String(UpdateProduit.ImgContent);
								}
							}
						}

						HaveUpdateImage = true;
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
				HaveUpdateImage = false;
			}
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
