using Crematorium.Domain.Abstractions;
using Crematorium.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crematorium.Application.Services
{
    public class CorposeService : HelpersService<Corpose>
    {
        public CorposeService(IUnitOfWork unitOfWork)
        {
            _repository = unitOfWork.CorposeRepository;
        }
    }
}
