using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessData.Models
{
	public class Rack
	{
		public string NomDuRack { get; set; }

		public string Position { get; set; }

		public string NomClient { get; set; }

		public int NumeroCommande { get; set; }

		public DateTime Date { get; set; }
	}
}
