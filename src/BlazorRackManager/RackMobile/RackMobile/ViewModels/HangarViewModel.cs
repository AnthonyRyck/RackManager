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

		/// <summary>
		/// Indicateur si l'utilisateur a le rôle requis pour voir cette
		/// ressource
		/// </summary>
		public bool HasGoodRole
		{
			get { return _hasGoodRole; }
			set
			{
				_hasGoodRole = value;
				OnNotifyPropertyChanged();
			}
		}
		private bool _hasGoodRole;

		/// <summary>
		/// Indicateur si l'utilisateur n'a pas le rôle requis pour voir cette
		/// ressource
		/// </summary>
		public bool NotGoodRole
		{
			get { return _notGoodRole; }
			set
			{
				_notGoodRole = value;
				OnNotifyPropertyChanged();
			}
		}
		private bool _notGoodRole;

		
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

		public void Init()
		{
			HasGoodRole = DetermineRole("Admin", "Manager", "Member");
			NotGoodRole = !HasGoodRole;
		}

		
	}
}
