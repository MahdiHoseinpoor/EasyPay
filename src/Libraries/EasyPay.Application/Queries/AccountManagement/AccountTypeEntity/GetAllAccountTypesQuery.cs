using EasyPay.Application.Common;
using EasyPay.Shared.DTOs.AccountManagement;
using MediatR;
using System.Collections.Generic;

namespace EasyPay.Application.Queries.AccountManagement.AccountTypeEntity
{
    public class GetAllAccountTypesQuery : IRequest<Result<List<AccountTypeDto>>>, IAuthorizableRequest<Result<List<AccountTypeDto>>>
    {
        public string RequiredPermission => Permissions.AccountTypes.View;
    }
}