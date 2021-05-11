using RackCore;
using RackMobile.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RackMobile.ViewModels
{
	public class RackVideViewModel : BaseViewModel
	{
		private IRackService RackService => DependencyService.Get<IRackService>();

		public List<Rack> Racks
		{
			get { return _racks; }
			set
			{
				_racks = value;
				OnNotifyPropertyChanged();
			}
		}
		private List<Rack> _racks = new List<Rack>();


		public RackVideViewModel()
		{
			Title = "Liste des racks vides";
		}


		public async Task LoadRacksVides()
		{
			Racks = await RackService.GetRacksEmpty();
		}

	}
}
