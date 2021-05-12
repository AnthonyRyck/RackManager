using RackCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

		private List<Rack> AllRacks = new List<Rack>();

		public RackOccupeViewModel()
		{
			Title = "Liste des racks occupés";
		}

		internal async Task LoadRacksFull()
		{
			AllRacks = await RackService.GetRacksOqp();

			Racks = new List<Rack>(AllRacks);
		}


		internal void Rechercher(string debutGisement)
		{
			Racks = AllRacks.Where(x => x.GisementPos.StartsWith(debutGisement))
							.ToList();
		}
	}
}
