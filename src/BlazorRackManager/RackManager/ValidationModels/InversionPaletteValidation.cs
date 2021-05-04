using System.ComponentModel.DataAnnotations;

namespace RackManager.ValidationModels
{
	public class InversionPaletteValidation
	{
		[Required(ErrorMessage = "Il faut choisir un emplacement")]
		[MinLength(1, ErrorMessage = "Il faut choisir un emplacement")]
		public string GisementRackPartant { get; set; }

		public int IdRackPartant { get; set; }


		[Required(ErrorMessage = "Il faut choisir un emplacement")]
		[MinLength(1, ErrorMessage = "Il faut choisir un emplacement")]
		public string GisementRackArrivant { get; set; }

		public int IdRackArrivant { get; set; }


		public int IdCommandeArrivant { get; set; }

		public int IdCommandePartant { get; set; }
	}
}
