using Crematorium.Domain.Abstractions;
using Crematorium.Domain.Entities;
using Crematorium.Persistense.Data;

namespace Crematorium.Persistense.Repository
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly CrematoriumDbContext _context;

        private readonly Lazy<IRepository<User>> _userRepository;
        private readonly Lazy<IRepository<Order>> _orderRepository;
        private readonly Lazy<IRepository<Corpose>> _corposeRepository;
        private readonly Lazy<IRepository<RitualUrn>> _ritualUrnRepository;
        private readonly Lazy<IRepository<Hall>> _hallRepository;

        public EfUnitOfWork(CrematoriumDbContext dbContext)
        {
            _context = dbContext;
            _userRepository = new Lazy<IRepository<User>>(() =>
                                    new EfRepository<User>(dbContext));
            _orderRepository = new Lazy<IRepository<Order>>(() =>
                                    new EfRepository<Order>(dbContext));
            _corposeRepository = new Lazy<IRepository<Corpose>>(() =>
                                    new EfRepository<Corpose>(dbContext));
            _ritualUrnRepository = new Lazy<IRepository<RitualUrn>>(() =>
                                    new EfRepository<RitualUrn>(dbContext));
            _hallRepository = new Lazy<IRepository<Hall>>(() =>
                                    new EfRepository<Hall>(dbContext));
        }

        public IRepository<User> UserRepository => _userRepository.Value;

        public IRepository<Order> OrderRepository => _orderRepository.Value;

        public IRepository<Corpose> CorposeRepository => _corposeRepository.Value;

        public IRepository<RitualUrn> RitualUrnRepository => _ritualUrnRepository.Value;

        public IRepository<Hall> HallRepository => _hallRepository.Value;

        public async Task CreateDatabaseAsync()
        {
            await _context.Database.EnsureCreatedAsync();
        }

        public async Task RemoveDatbaseAsync()
        {
            await _context.Database.EnsureDeletedAsync();
        }

        public async Task SaveAllAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
