using EasyPay.Common;
using EasyPay.Domain.Entities.Report;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Infrastructure.Aggregates.Report
{
    public class TransactionRepository : RepositoryBase<Transaction, Guid>,ITransactionRepository
    {
        public TransactionRepository(DbContext context) : base(context)
        {
        }
    }
}
