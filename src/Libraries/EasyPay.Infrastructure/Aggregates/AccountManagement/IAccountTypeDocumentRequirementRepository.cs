using EasyPay.Common;
using EasyPay.Domain.Entities.AccountManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Infrastructure.Aggregates.AccountManagement
{
    public interface IAccountTypeDocumentRequirementRepository : IRepositoryBase<AccountTypeDocumentRequirement, int>
    {
    }
}
