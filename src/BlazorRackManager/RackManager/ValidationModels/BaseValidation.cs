using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ValidationModels
{
	public class BaseValidation
	{
		[Required(ErrorMessage = "Il faut choisir un emplacement")]
		[MinLength(1)]
		public string GisementRack { get; set; }

		public int IdRack { get; set; }
	}
}
