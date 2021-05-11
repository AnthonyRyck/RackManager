using RackMobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace RackMobile.ViewModels
{
	public class HangarViewModel : BaseViewModel
	{
		public List<ItemHangar> HangarItems
		{
			get { return _hangarItems; }
			set
			{
				_hangarItems = value;
				OnNotifyPropertyChanged();
			}
		}
		private List<ItemHangar> _hangarItems;



		public HangarViewModel()
		{
			Title = "Hangar";

			HangarItems = new List<ItemHangar>()
			{
				new ItemHangar()
				{
					ChoixDisplay = "Liste des racks vides",
					ChoixMenu = "RacksVide"
				},
				new ItemHangar()
				{
					ChoixDisplay = "Liste des racks occupés",
					ChoixMenu = "RacksOqp"
				},
			};
		}

	}
}
