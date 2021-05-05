using System;

namespace AccessData.Views
{
	public class CommandeSortieView
	{
		public int IdCommande { get; set; }

		public string DescriptionCmd { get; set; }

		public int IdClient { get; set; }

		public string NomClient { get; set; }

		public DateTime DateSortie { get; set; }
	}
}
