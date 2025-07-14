using EasyPay.Common;
using EasyPay.Domain.Entities.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Infrastructure.Aggregates.Report
{
    public interface ITransactionRepository :IRepositoryBase<Transaction,Guid>
    {
    }
}
