using System;

namespace RackCore
{
	public class SuiviCommande
	{
		public int IdCommande { get; set; }

		public int ClientId { get; set; }

		public string DescriptionCmd { get; set; }

		public DateTime? DateSortie { get; set; }
	}
}
