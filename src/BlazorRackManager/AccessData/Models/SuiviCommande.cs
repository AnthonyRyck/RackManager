namespace AccessData.Models
{
	public class SuiviCommande
	{
		public int IdCommande { get; set; }

		public int ClientId { get; set; }

		public string DescriptionCmd { get; set; }

		public string IdCommandeStr { get { return IdCommande.ToString(); } }

	}
}
