using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSIL.Models
{
	public class CarsVariant
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public CarSpecs Specs { get; set; }
	}
}