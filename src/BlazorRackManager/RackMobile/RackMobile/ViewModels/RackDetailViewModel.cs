using RackCore;
using RackCore.EntityView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RackMobile.ViewModels
{
	public class RackDetailViewModel : BaseViewModel
	{
		private Rack RackSelected;

		public HangarView RackInfo
		{
			get { return _rackInfo; }
			set
			{
				_rackInfo = value;
				OnNotifyPropertyChanged();
			}
		}
		private HangarView _rackInfo;


		public RackDetailViewModel(Rack rackSelected)
		{
			RackSelected = rackSelected;
		}

		internal async Task LoadInfoRack()
		{
			RackInfo = await RackService.GetInfoRack(RackSelected.IdRack);
		}
	}
}
