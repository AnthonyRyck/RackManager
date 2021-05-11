using RackMobile.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace RackMobile.Views
{
	public partial class ItemDetailPage : ContentPage
	{
		public ItemDetailPage()
		{
			InitializeComponent();
			BindingContext = new ItemDetailViewModel();
		}
	}
}