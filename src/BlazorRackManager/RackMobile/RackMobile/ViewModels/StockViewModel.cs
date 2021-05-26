using RackMobile.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RackMobile.ViewModels
{
	class StockViewModel : BaseViewModel
	{
		public List<ItemChoice> Choix
		{
			get { return _choix; }
			set
			{
				_choix = value;
				OnNotifyPropertyChanged();
			}
		}
		private List<ItemChoice> _choix;

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


		public StockViewModel()
		{
			Title = "Stock";

			Choix = new List<ItemChoice>()
			{
				new ItemChoice()
				{
					ChoixDisplay = "Chercher avec une référence",
					ChoixMenu = "RefProduit"
				},
				new ItemChoice()
				{
					ChoixDisplay = "Choisir un rack de stock",
					ChoixMenu = "RacksStock"
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
