using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.Data
{
	public class SettingEntity
	{
		/// <summary>
		/// Nombre de jours avant de supprimer les logs.
		/// </summary>
		public uint NombreJoursToDeleteLogs { get; set; }
	}
}
