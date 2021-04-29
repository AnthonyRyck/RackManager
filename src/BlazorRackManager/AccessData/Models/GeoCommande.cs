using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessData.Models
{
	public class GeoCommande
	{
		public int RackId { get; set; }
		public int CommandeId { get; set; }
		public DateTime DateEntree { get; set; }
	}
}
