using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crematorium.Domain.Entities
{
    public class Ceremony : Base
    {
        public string Contact { get; set; }
        public string NameOfCompany { get; set; }
        public string Description { get; set; }
    }
}
