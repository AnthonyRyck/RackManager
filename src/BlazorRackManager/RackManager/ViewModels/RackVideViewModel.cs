using AccessData;
using AccessData.Models;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public class RackVideViewModel : IRackVideViewModel
	{
		public bool IsLoaded { get; set; }

		public IEnumerable<Rack> EmptyRacks { get; set; }

		private SqlContext SqlContext;
		private NotificationService Notification;

		public RackVideViewModel(SqlContext sqlContext, NotificationService notification)
		{
			SqlContext = sqlContext;
			Notification = notification;
		}


		public async Task LoadEmptyRacks()
		{
			try
			{
				IsLoaded = false;

				EmptyRacks = await SqlContext.GetRackEmpty();
			}
			catch (Exception)
			{
				Notification.Notify(NotificationSeverity.Error, "Erreur chargement", "Erreur sur le chargement des racks vides");
			}

			IsLoaded = true;
		}
	}
}
