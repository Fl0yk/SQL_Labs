using Crematorium.Application.Abstractions;
using Crematorium.Domain.Abstractions;
using Crematorium.Domain.Entities;
using Crematorium.Persistense.Repository.Postgre;
using System.Linq.Expressions;

namespace Crematorium.Application.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        private UsersRepository repository;
        public UserService(IUnitOfWork unitOfWork, UsersRepository usersRepository)
        {
            _repository = unitOfWork.UserRepository;
            repository = usersRepository;
            //_repository.AddAsync(new User() { Id = 1, Name = "Admin", MailAdress = "admin@mail.ru", NumPassport = "1111111A111PB1", Surname = "Adminov", UserRole = Role.Admin });
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _repository.ListAllAsync(CancellationToken.None, u => u.Orders);
        }

        public override async Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> filter)
        {
            return await _repository.FirstOrDefaultAsync(filter, CancellationToken.None, u => u.Orders);
        }

        /// <summary>
        /// Проверяет наличие пользователя по имени и номеру паспорта. Возвращает true, если пользователь существует
        /// </summary>
        public Task<bool> IsValided(string name, string numPassport)
        {
            return repository.IsExist(name, numPassport);
            var item = _repository.FirstOrDefaultAsync(u => u.Name == name && u.NumPassport == numPassport).Result;
            if (item  == default)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        } 

        public async Task<IEnumerable<User>> FindByName(string name)
        {
            return await _repository.ListAsync(u => u.Name == name, CancellationToken.None, u => u.Orders);
        }
    }
}
