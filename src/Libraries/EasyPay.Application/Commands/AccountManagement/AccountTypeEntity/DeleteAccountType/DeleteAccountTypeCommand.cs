using EasyPay.Application.Common;
using MediatR;
using System.Text.Json.Serialization;

namespace EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.DeleteAccountType
{
    public class DeleteAccountTypeCommand : IRequest<Result>, IAuthorizableRequest<Result>
    {
        public int Id { get; set; }
        public bool IsHardDelete { get; set; }

        [JsonIgnore]
        public string RequiredPermission => IsHardDelete
            ? Permissions.AccountTypes.HardDelete
            : Permissions.AccountTypes.Delete;
    }
}