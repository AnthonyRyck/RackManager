using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ValidationModels
{
	public class UniteMesureValidation
	{
		[Required(ErrorMessage = "Il faut un nom d'unité de mesure")]
		[StringLength(10, ErrorMessage = "Le nom de l'unité est trop longue, 10 caractères max")]
		public string NomMesure { get; set; }
	}
}
