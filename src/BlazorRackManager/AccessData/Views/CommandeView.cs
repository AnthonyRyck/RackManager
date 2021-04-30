using AccessData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessData.Views
{
	public class CommandeView
	{
		public int IdCommande { get; set; }

		public string DescriptionCmd { get; set; }

		public int IdClient { get; set; }

		public string NomClient { get; set; }
	}
}
