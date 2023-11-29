using Crematorium.Application.Abstractions;
using Crematorium.Domain.Abstractions;
using Crematorium.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crematorium.Application.Services
{
    public class RitualUrnService : HelpersService<RitualUrn>
    {
       public RitualUrnService(IUnitOfWork unitOfWork) 
       {
            _repository = unitOfWork.RitualUrnRepository;
       }
    }
}
