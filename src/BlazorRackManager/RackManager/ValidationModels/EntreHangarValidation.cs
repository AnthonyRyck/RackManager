using RackCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace RackManager.ValidationModels
{
	public class EntreHangarValidation : BaseValidation
	{

		[Required(ErrorMessage ="Il faut une date d'entrée")]
		public DateTime? DateEntree { get; set; }

		[Required(ErrorMessage = "Il faut un numéro de commande")]
		public int? IdCommande { get; set; }

		[Required(ErrorMessage = "Il faut un client")]
		public int? IdClient { get; set; }

		[Required(ErrorMessage = "Il faut un client")]
		[MinLength(1, ErrorMessage = "Il faut un client")]
		public string NomClient { get; set; }

		public string Description { get; set; }


		internal SuiviCommande ToSuiviCommande()
		{
			return new SuiviCommande()
			{
				ClientId = IdClient.Value,
				IdCommande = IdCommande.Value,
				DescriptionCmd = Description
			};
		}

		internal GeoCommande ToGeoCommande()
		{
			return new GeoCommande()
			{
				RackId = IdRack,
				CommandeId = IdCommande.Value,
				DateEntree = DateTime.Now
			};
		}
	}
}
