using System;
using System.Collections.Generic;
using System.Text;

namespace RackCore
{
	public class LogEntity
	{
		public DateTime DateLog { get; set; }

		public string Level { get; set; }

		public string Message { get; set; }

		public string ExceptionMsg { get; set; }

	}
}
