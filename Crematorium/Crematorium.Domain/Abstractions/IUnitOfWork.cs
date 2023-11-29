using Crematorium.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crematorium.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        IRepository<User> UserRepository { get; }
        IRepository<Order> OrderRepository { get; }
        IRepository<Corpose> CorposeRepository { get; }
        IRepository<RitualUrn> RitualUrnRepository { get; }
        IRepository<Hall> HallRepository { get; }
        public Task RemoveDatbaseAsync();
        public Task CreateDatabaseAsync();
        public Task SaveAllAsync();
    }
}
