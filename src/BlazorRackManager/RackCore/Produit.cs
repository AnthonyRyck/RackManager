using System;
using System.Collections.Generic;
using System.Text;

namespace RackCore
{
	public class Produit
	{
		public string IdReference { get; set; }

		public string Nom { get; set; }

		public int UniteId { get; set; }

		public string ImageName { get; set; }

		public byte[] ImageContent { get; set; }
	}
}
