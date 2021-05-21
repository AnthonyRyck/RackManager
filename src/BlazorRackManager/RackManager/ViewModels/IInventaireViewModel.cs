using AccessData.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public interface IInventaireViewModel
	{
		bool IsLoaded { get; }

		List<InventaireView> AllProduits { get; }



		Task LoadInventaire();
	}
}
