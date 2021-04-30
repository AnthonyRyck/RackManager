using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessData.Views
{
	public class HangarView
	{
		public int IdClient { get; set; }

		public string NomClient { get; set; }

		public int IdRack { get; set; }

		public string Gisement { get; set; }

		public string PosRack { get; set; }

		public int IdCommande { get; set; }

		public int ClientId { get; set; }

		public string DescriptionCmd { get; set; }
	}
}
