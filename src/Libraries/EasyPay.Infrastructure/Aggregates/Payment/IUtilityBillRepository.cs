using EasyPay.Common;
using EasyPay.Domain.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Infrastructure.Aggregates.Payment
{
    public interface IUtilityBillRepository : IRepositoryBase<UtilityBill, Guid>
    {
    }
}
