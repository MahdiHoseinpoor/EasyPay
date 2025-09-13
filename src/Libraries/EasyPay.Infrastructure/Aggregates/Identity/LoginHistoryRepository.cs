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
    public class VerifyPasswordHistoryRepository : RepositoryBase<VerifyPasswordHistory, long>, IVerifyPasswordHistoryRepository
    {
        public VerifyPasswordHistoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
