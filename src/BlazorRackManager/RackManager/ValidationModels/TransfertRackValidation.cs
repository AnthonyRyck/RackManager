using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ValidationModels
{
	public class TransfertRackValidation
	{
		[Required(ErrorMessage = "Il faut choisir un emplacement")]
		[MinLength(1, ErrorMessage = "Il faut choisir un emplacement")]
		public string GisementRackPartant { get; set; }

		public int IdRackPartant { get; set; }


		[Required(ErrorMessage = "Il faut choisir un emplacement")]
		[MinLength(1, ErrorMessage = "Il faut choisir un emplacement")]
		public string GisementRackArrivant { get; set; }

		public int IdRackArrivant { get; set; }
	}
}
