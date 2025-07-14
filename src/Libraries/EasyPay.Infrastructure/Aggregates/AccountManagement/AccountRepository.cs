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
    public class AccountRepository : RepositoryBase<Account, Guid>, IAccountRepository
    {
        public AccountRepository(DbContext context) : base(context)
        {
        }
    }
}
