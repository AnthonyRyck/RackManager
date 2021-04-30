using System.ComponentModel.DataAnnotations;

namespace RackManager.ValidationModels
{
	public class RackValidation
	{
		[Required(ErrorMessage = "Il faut un gisement")]
		[StringLength(5, ErrorMessage = "Gisement trop long, 5 caractères max")]
		public string Gisement { get; set; }

		[Required(ErrorMessage = "Il faut une position dans le rack : A, B, C,...")]
		[StringLength(1, ErrorMessage = "La position trop long, 1 caractère max")]
		public string Position { get; set; }
	}
}
