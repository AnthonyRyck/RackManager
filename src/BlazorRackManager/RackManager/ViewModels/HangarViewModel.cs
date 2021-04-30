using AccessData;
using AccessData.Views;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public class HangarViewModel : IHangarViewModel
	{
		public bool IsLoaded { get; set; }

		public bool DialogNouvelleEntre { get; set; }

		public List<HangarView> AllHangar { get; set; }

		public RadzenGrid<HangarView> HangarGrid { get; set; }

		private SqlContext SqlContext;
		private NotificationService Notification;

		public HangarViewModel(SqlContext sqlContext, NotificationService notification)
		{
			SqlContext = sqlContext;
			Notification = notification;
			IsLoaded = false;
			DialogNouvelleEntre = false;

			AllHangar = LoadDatas().GetAwaiter().GetResult();

			IsLoaded = true;
		}

		#region Public methods

		public void OpenNouvelleEntre()
		{
			DialogNouvelleEntre = true;
		}

		#endregion

		#region Private methods

		private async Task<List<HangarView>> LoadDatas()
		{
			List<HangarView> hangarViews = new List<HangarView>();

			try
			{
				hangarViews = await SqlContext.GetHangar();
			}
			catch (Exception ex)
			{
				Notification.Notify(NotificationSeverity.Error, "Erreur chargement", "Erreur sur le chargement des informations du hangar");
			}

			return hangarViews;
		}

		#endregion
	}
}
