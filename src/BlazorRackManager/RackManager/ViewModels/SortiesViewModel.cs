using AccessData;
using AccessData.Views;
using Radzen.Blazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public class SortiesViewModel : ISortiesViewModel
	{
		public bool IsLoaded { get; set; }

		public IEnumerable<CommandeSortieView> Sorties { get; set; }


		private SqlContext SqlContext;

		public SortiesViewModel(SqlContext sqlContext)
		{
			SqlContext = sqlContext;
			IsLoaded = false;
		}


		public async Task LoadDatas()
		{
			Sorties = await SqlContext.GetSorties();
			IsLoaded = true;
		}

	}
}
