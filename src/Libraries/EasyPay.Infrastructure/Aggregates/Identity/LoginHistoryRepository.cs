using EasyPay.Common;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Infrastructure.Aggregates.Identity
{
    public class LoginHistoryRepository : RepositoryBase<LoginHistory, long>, ILoginHistoryRepository
    {
        public LoginHistoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
