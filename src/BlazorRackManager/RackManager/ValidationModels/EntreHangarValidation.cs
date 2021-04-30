using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ValidationModels
{
	public class EntreHangarValidation
	{
		[Required(ErrorMessage = "Il faut choisir un emplacement")]
		[MinLength(1)]
		public string GisementRack { get; set; }

		[Required(ErrorMessage ="Il faut une date d'entrée")]
		public DateTime? DateEntree { get; set; }

		[Required(ErrorMessage = "Il faut un numéro de commande")]
		public int? CommandeId { get; set; }
	}
}
