using RackCore;
using System.ComponentModel.DataAnnotations;

namespace RackManager.ValidationModels
{
	public class ProduitValidation
	{
		[Required(ErrorMessage = "Il faut une référence")]
		[StringLength(25, ErrorMessage = "La référence est trop longue, 25 caractères max")]
		public string Reference { get; set; }

		[Required(ErrorMessage = "Il faut un nom de produit")]
		[StringLength(50, ErrorMessage = "Le nom du produit est trop long, 50 caractères max")]
		public string NomProduit { get; set; }

		[Required(ErrorMessage = "Il faut une unité de mesure")]
		public int? IdMesure { get; set; }

		public byte[] ImgContent { get; set; }

		public Produit ToProduit()
		{
			return new Produit()
			{
				IdReference = Reference,
				Nom = NomProduit,
				UniteId = IdMesure.Value,
				ImageName = Reference + ".png",
				ImageContent = ImgContent
			};
		}
	}
}
