using RackCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public interface IRackVideViewModel
	{
		bool IsLoaded { get; set; }

		IEnumerable<Rack> EmptyRacks { get; set; }


		Task LoadEmptyRacks();
	}
}
