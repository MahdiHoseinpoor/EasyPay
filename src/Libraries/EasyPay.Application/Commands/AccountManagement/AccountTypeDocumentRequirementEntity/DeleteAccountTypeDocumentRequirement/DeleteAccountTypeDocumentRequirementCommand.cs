using EasyPay.Application.Common;
using MediatR;
using System.Text.Json.Serialization;

namespace EasyPay.Application.Commands.AccountManagement.AccountTypeDocumentRequirementEntity.DeleteAccountTypeDocumentRequirement
{
    public class DeleteAccountTypeDocumentRequirementCommand : IRequest<Result>, IAuthorizableRequest<Result>
    {
        [JsonIgnore]
        public int AccountTypeId { get; set; }

        [JsonIgnore]
        public int RequirementId { get; set; }

        public string RequiredPermission => Permissions.AccountTypes.Edit; // Deleting a requirement is considered an "Edit" action on the Account Type
    }
}