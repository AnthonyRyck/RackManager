using RackCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RackMobile.ViewModels
{
	public class RackOccupeViewModel : BaseViewModel
	{
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


		public RackOccupeViewModel()
		{
			Title = "Liste des racks occupés";
		}

		internal async Task LoadRacksFull()
		{
			Racks = await RackService.GetRacksOqp();
		}

	}
}
