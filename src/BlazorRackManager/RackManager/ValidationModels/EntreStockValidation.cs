using RackCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace RackManager.ValidationModels
{
	public class EntreStockValidation
	{
		[Required(ErrorMessage = "Il faut un produit")]
		[MinLength(1, ErrorMessage = "Il faut un produit")]
		public string ReferenceProduit { get; set; }

		[Required(ErrorMessage = "Il faut choisir un emplacement")]
		[MinLength(1)]
		public string GisementRack { get; set; }

		[Required(ErrorMessage = "Il faut indiquer une quantité")]
		[Range(0.1, double.MaxValue, ErrorMessage = "Il faut une valeur supérieur à zéro")]
		public double Quantite { get; set; }


		public int IdRack { get; set; }

		internal Stock ToStock()
		{
			return new Stock()
			{
				ProduitId = ReferenceProduit,
				Quantite = Quantite,
				RackId = IdRack
			};
		}
	}
}
