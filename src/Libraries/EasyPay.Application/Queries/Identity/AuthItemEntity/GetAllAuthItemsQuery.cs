using EasyPay.Application.Common;
using EasyPay.Shared.DTOs.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Queries.Identity.AuthItemEntity
{
    public class GetAllAuthItemsQuery : IAuthorizableRequest<Result<List<AuthItemDto>>>
    {
        public string RequiredPermission => Permissions.AuthItem.View;
    }
}
