using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ValidationModels
{
	public class ClientValidation
	{
		[Required(ErrorMessage = "Il faut un nom de client")]
		[StringLength(50, ErrorMessage = "Le nom est trop long, 50 caractères max")]
		public string NomClient { get; set; }
	}
}
