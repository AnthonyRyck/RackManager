using AccessData;
using AccessData.Views;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public class InventaireViewModel : IInventaireViewModel
	{
		public bool IsLoaded { get; private set; }

		public List<InventaireView> AllProduits { get; private set; }



		private SqlContext ContextSql;

		public InventaireViewModel(SqlContext sqlContext)
		{
			ContextSql = sqlContext;
		}



		public async Task LoadInventaire()
		{
			try
			{
				IsLoaded = false;
				AllProduits = await ContextSql.GetInventaire();
				IsLoaded = true;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Erreur sur le chargement de l'inventaire");
			}
		}

	}
}
