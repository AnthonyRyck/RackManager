using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessData.Views
{
	public class ProduitView
	{
		public string IdReference { get; set; }

		public string Nom { get; set; }

		public string UniteMesure { get; set; }

		public int IdMesure { get; set; }

		public byte[] ImageProduit { get; set; }

		public string Base64String { 
			get
			{
				if(ImageProduit != null)
				{
					return "data:image/png;base64," + Convert.ToBase64String(ImageProduit);
				}
				return string.Empty;
			}
		}

	}
}
