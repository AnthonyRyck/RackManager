using AccessData.Models;
using RackManager.ValidationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	interface INewClientViewModel
	{
		ClientValidation NouveauClient { get; set; }


		void OnValidSubmit();


		void ClosePage();
	}
}
