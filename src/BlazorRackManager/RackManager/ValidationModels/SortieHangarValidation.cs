using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ValidationModels
{
	public class SortieHangarValidation : BaseValidation
	{
		[Required(ErrorMessage = "Il faut un numéro de commande")]
		public int? IdCommande { get; set; }

		[Required(ErrorMessage = "Il faut une date de sortie")]
		public DateTime? DateSortie { get; set; }
	}
}
