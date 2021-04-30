using AccessData.Views;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public interface IHangarViewModel
	{
		bool IsLoaded { get; set; }

		bool DialogNouvelleEntre { get; set; }

		List<HangarView> AllHangar { get; set; }

		RadzenGrid<HangarView> HangarGrid { get; set; }



		void OpenNouvelleEntre();
	}
}
