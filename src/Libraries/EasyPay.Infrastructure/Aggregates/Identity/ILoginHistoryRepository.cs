using EasyPay.Common;
using EasyPay.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Infrastructure.Aggregates.Identity
{
    public interface ILoginHistoryRepository : IRepositoryBase<LoginHistory,long>
    {
    }
}
