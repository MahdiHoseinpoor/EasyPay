using EasyPay.Application.Common;
using MediatR;
using System.Text.Json.Serialization;

namespace EasyPay.Application.Commands.AccountManagement.AccountTypeDocumentRequirementEntity.CreateAccountTypeDocumentRequirement
{
    public class CreateAccountTypeDocumentRequirementCommand : IRequest<Result<int>>, IAuthorizableRequest<Result<int>> 
    {
        [JsonIgnore]
        public int AccountTypeId { get; set; }

        public int AuthItemId { get; set; }

        public bool IsRequired { get; set; } = true;

        public int? Order { get; set; }

        public string RequiredPermission => Permissions.AccountTypes.Edit;
    }
}