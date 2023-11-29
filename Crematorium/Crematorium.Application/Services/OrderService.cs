using Crematorium.Application.Abstractions;
using Crematorium.Domain.Abstractions;
using Crematorium.Domain.Entities;
using System.Linq.Expressions;

namespace Crematorium.Application.Services
{
    public class OrderService : BaseService<Order>, IOrderService
    {
        public OrderService(IUnitOfWork unitOfWork) 
        {
            _repository = unitOfWork.OrderRepository;
        }

        public override async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _repository.ListAllAsync(CancellationToken.None, o => o.RitualUrnId, o => o.DateOfStart, o => o.Customer, o => o.HallId, o => o.CorposeId);
        }

        public virtual async Task<IEnumerable<Order>> GetListAsync(Expression<Func<Order, bool>> filter)
        {
            return await _repository.ListAsync(filter, CancellationToken.None, o => o.RitualUrnId, o => o.DateOfStart, o => o.Customer, o => o.HallId, o => o.CorposeId);
        }

        public override async Task<Order?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id, CancellationToken.None, o => o.RitualUrnId, o => o.DateOfStart, o => o.Customer, o => o.HallId, o => o.CorposeId);
        }

        public override async Task<Order?> FirstOrDefaultAsync(Expression<Func<Order, bool>> filter)
        {
            return await _repository.FirstOrDefaultAsync(filter, CancellationToken.None, o => o.RitualUrnId, o => o.DateOfStart, o => o.Customer, o => o.HallId, o => o.CorposeId);
        }

        public async Task CancelOrder(int Id)
        {
            var order = _repository.GetByIdAsync(Id).Result;
            await CancelOrder(order);

            return;
        }

        public async Task CancelOrder(Order? order)
        {
            if (order is null || order.State == StateOrder.Closed)
                return;

            order.State = StateOrder.Cancelled;
            await _repository.UpdateAsync(order);

            return;
        }

        public async Task NextState(int Id)
        {
            var order = _repository.GetByIdAsync(Id).Result;
            await NextState(order);

            return;
        }

        public async Task NextState(Order? order)
        {
            if (order is null)
                return;

            switch (order.State)
            {
                case StateOrder.Decorated:
                    order.State = StateOrder.Approved;
                    break;

                case StateOrder.Approved:
                    order.State = StateOrder.Closed;
                    break;
            }
            await _repository.UpdateAsync(order);

            return;
        }
    }
}
