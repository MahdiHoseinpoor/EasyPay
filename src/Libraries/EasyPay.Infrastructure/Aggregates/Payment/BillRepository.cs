using EasyPay.Common;
using EasyPay.Domain.Entities.Payment;
using EasyPay.Infrastructure.Data;

namespace EasyPay.Infrastructure.Aggregates.Payment
{
    public class BillRepository : RepositoryBase<BillBase, Guid>, IBillRepository
    {
        public BillRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}