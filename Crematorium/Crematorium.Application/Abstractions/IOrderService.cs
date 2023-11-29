using Crematorium.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crematorium.Application.Abstractions
{
    public interface IOrderService : IBaseService<Order>
    {
        public Task NextState(int Id);

        public Task NextState(Order? order);

        public Task CancelOrder(int Id);

        public Task CancelOrder(Order? order);
    }
}
