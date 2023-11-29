using Crematorium.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crematorium.Application.Abstractions
{
    public interface IUserService : IHelpersService<User>
    {
        Task<bool> IsValided(string name, string numPassport);
    }
}
