using RackMobile.ViewModels;
using RackMobile.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace RackMobile
{
	public partial class AppShell : Xamarin.Forms.Shell
	{
		public AppShell()
		{
			InitializeComponent();
			//Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
			//Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
		}

	}
}
