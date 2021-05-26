using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RackMobile.Views.Stock
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProduitStockPage : ContentPage
	{
		public ProduitStockPage()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Permet de rechercher la référence du produit donné.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Recherche_Completed(object sender, EventArgs e)
		{
			string referenceProduit = (sender as Entry).Text;
			await ViewModel.GetStockByProduit(referenceProduit);
		}

	}
}