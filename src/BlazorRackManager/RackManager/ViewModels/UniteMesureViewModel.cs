using AccessData;
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
	public class UniteMesureViewModel : IUniteMesureViewModel
	{
		#region Properties

		public bool IsLoaded { get; private set; }

		public bool CanOpenNewUnite { get; private set; }

		public UniteMesureValidation UniteMesure { get; private set; }

		public List<UniteMesure> AllMesures { get; private set; }

		public RadzenGrid<UniteMesure> UniteGrid { get; set; }

		private SqlContext ContextSql;
		private NotificationService Notification;

		#endregion

		public UniteMesureViewModel(SqlContext sqlContext, NotificationService notificationService)
		{
			ContextSql = sqlContext;
			Notification = notificationService;

			IsLoaded = false;
			CanOpenNewUnite = false;
			UniteMesure = new UniteMesureValidation();
		}


		#region Public Methods


		public async Task LoadUnites()
		{
			try
			{
				AllMesures = await ContextSql.GetUniteMesure();
				IsLoaded = true;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "UniteMesureViewModel - LoadUnites");
				Notification.Notify(NotificationSeverity.Error, "Erreur", "Erreur sur le chargement");
			}
		}


		public void OpenNewUnite()
		{
			CanOpenNewUnite = true;
		}

		public void CloseNewMesure()
		{
			CanOpenNewUnite = false;
		}

		public async Task OnValidSubmitMesure()
		{
			try
			{
				// Si contient déjà le même nom de client.
				if (AllMesures.Any(x => x.Unite == UniteMesure.NomMesure))
				{
					string msgWarn = $"Aucun ajout : {UniteMesure.NomMesure} existe déjà";
					NotificationMessage messWarn = new NotificationMessage()
					{
						Summary = "Attention",
						Detail = msgWarn,
						Duration = 3000,
						Severity = NotificationSeverity.Warning
					};
					Notification.Notify(messWarn);

					return;
				}

				// Ajout dans la base de donnée.
				int idMesure = await ContextSql.AddUniteMesure(UniteMesure.NomMesure);

				UniteMesure unite = new UniteMesure()
				{
					IdMesure = idMesure,
					Unite = UniteMesure.NomMesure
				};

				AllMesures.Add(unite);
				await UniteGrid.Reload();

				string message = $"Nouvelle unitée - {unite.Unite} ajoutée";
				NotificationMessage messNotif = new NotificationMessage()
				{
					Summary = "Sauvegarde OK",
					Detail = message,
					Duration = 3000,
					Severity = NotificationSeverity.Success
				};
				Notification.Notify(messNotif);

				Log.Information("UNITE - " + message);

				UniteMesure = new UniteMesureValidation();

			}
			catch (Exception ex)
			{
				Log.Error(ex, "UniteMesureViewModel - OnValidSubmitMesure");
				Notification.Notify(NotificationSeverity.Error, "Erreur", "Erreur sur la sauvegarde");
			}
		}

		#endregion

	}
}
