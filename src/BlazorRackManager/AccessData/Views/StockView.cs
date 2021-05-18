using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessData.Views
{
	public class StockView
	{
		public int IdRack { get; set; }

		public string Gisement { get; set; }

		public string PosRack { get; set; }

		public string GisementPos { get { return Gisement + "-" + PosRack; } }

		public string ReferenceProduit { get; set; }

		public string NomDuProduit { get; set; }

		public double Quantite { get; set; }

		public string Unite { get; set; }
	}
}
