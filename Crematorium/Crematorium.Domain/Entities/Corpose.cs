using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crematorium.Domain.Entities
{
    public class Corpose : Entity //труп
    {
        public string SurName { get; set; }

        public string NumPassport { get; set; }
    }
}
