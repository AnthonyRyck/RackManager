using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RackCore.EntityView
{
	public class HangarView
	{
		public int IdClient { get; set; }

		public string NomClient { get; set; }

		public int IdRack { get; set; }

		public string Gisement { get; set; }

		public string PosRack { get; set; }

		public int IdCommande { get; set; }

		public string DescriptionCmd { get; set; }
		
		public DateTime DateEntree { get; set; }
	}

}
