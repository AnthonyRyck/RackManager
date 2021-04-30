using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessData.Models
{
	public class Rack
	{
		public int IdRack { get; set; }

		public string Gisement { get; set; }

		public string PosRack { get; set; }


		public string GisementPos { get { return Gisement + "-" + PosRack; } }
	}
}
