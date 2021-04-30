using AccessData.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ValidationModels
{
	public class CommandeValidation
	{
		[Required(ErrorMessage = "Il faut un numéro de commande")]
		public int? IdCommande { get; set; }

		[Required(ErrorMessage = "Il faut un client")]
		public int? IdClient { get; set; }

		[StringLength(250, ErrorMessage = "La description est trop long, 250 caractères max")]
		public string DescriptionCmd { get; set; }

		[Required(ErrorMessage = "Il faut un client")]
		[MinLength(1, ErrorMessage = "Il faut un client")]
		public string NomClient { get; set; }


		internal SuiviCommande ToSuiviCommande()
		{
			return new SuiviCommande()
			{
				ClientId = IdClient.Value,
				IdCommande = IdCommande.Value,
				DescriptionCmd = DescriptionCmd
			};
		}
	}
}
