using EasyPay.Common;
using EasyPay.Domain.Entities.Payment;

namespace EasyPay.Infrastructure.Aggregates.Payment
{
    public interface IBillRepository : IRepositoryBase<BillBase, Guid>
    {
    }
}