using EasyPay.Application.Common;
using EasyPay.Shared.DTOs.AccountManagement;
using MediatR;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EasyPay.Application.Queries.AccountManagement.AccountTypeDocumentRequirementEntity
{
    public class GetAllAccountTypeDocumentRequirementsQuery : IRequest<Result<List<AccountTypeDocumentRequirementDto>>>, IAuthorizableRequest<Result<List<AccountTypeDocumentRequirementDto>>>
    {
        [JsonIgnore]
        public int AccountTypeId { get; set; }

        public string RequiredPermission => Permissions.AccountTypes.View;
    }
}