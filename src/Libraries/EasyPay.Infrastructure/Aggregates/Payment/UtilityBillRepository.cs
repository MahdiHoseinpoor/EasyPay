using EasyPay.Common;
using EasyPay.Domain.Entities.Payment;
using EasyPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Infrastructure.Aggregates.Payment
{
    public class UtilityBillRepository : RepositoryBase<UtilityBill, Guid>, IUtilityBillRepository
    {
        public UtilityBillRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
