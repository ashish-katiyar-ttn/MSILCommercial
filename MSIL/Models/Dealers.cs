using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSIL.Models
{
    public class Dealers
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Contacts { get; set; }
        public string Address { get; set; }
        public string Longitude { get; set; }
        public string Lattitude { get; set; }
    }
}