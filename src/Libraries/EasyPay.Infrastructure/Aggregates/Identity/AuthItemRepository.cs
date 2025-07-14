using EasyPay.Common;
using EasyPay.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Infrastructure.Aggregates.Identity
{
    public class AuthItemRepository : RepositoryBase<AuthItem, int>, IAuthItemRepository
    {
        public AuthItemRepository(DbContext context) : base(context)
        {
        }
    }
}
