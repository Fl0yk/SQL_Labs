using Crematorium.Domain.Abstractions;
using Crematorium.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Crematorium.Application.Services
{
    public class HallService : HelpersService<Hall>
    {
        public HallService(IUnitOfWork unitOfWork) {
            _repository = unitOfWork.HallRepository;
            ClearDate();
        }

        private void ClearDate()
        {
            foreach (var hall in _repository.ListAllAsync(CancellationToken.None, h => h.FreeDates).Result)
            {
                for (int i = 0; i < hall.FreeDates.Count; i++)
                {
                    var date = hall.FreeDates[i];
                    if ( DateTime.TryParse(date.Data, out DateTime hallDate))
                    {
                        if (hallDate.Date <  DateTime.Now.Date)
                        {
                            hall.FreeDates.Remove(date);
                            i--;
                        }
                    }
                }
                Task.FromResult(() => _repository.UpdateAsync(hall));
            }
            
        }

        public override async Task<IEnumerable<Hall>> GetAllAsync()
        { 
            return await _repository.ListAllAsync(CancellationToken.None, h => h.FreeDates);
        }
    }
}
