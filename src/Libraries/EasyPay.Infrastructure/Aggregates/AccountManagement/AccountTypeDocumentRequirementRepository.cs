using EasyPay.Common;
using EasyPay.Domain.Entities.AccountManagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Infrastructure.Aggregates.AccountManagement
{
    public class AccountTypeDocumentRequirementRepository : RepositoryBase<AccountTypeDocumentRequirement, int>, IAccountTypeDocumentRequirementRepository
    {
        public AccountTypeDocumentRequirementRepository(DbContext context) : base(context)
        {
        }
    }
}
