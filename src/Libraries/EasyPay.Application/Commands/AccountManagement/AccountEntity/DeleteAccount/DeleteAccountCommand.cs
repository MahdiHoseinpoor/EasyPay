using EasyPay.Application.Common;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace EasyPay.Application.Commands.AccountManagement.AccountEntity.DeleteAccount
{
    public class DeleteAccountCommand : IRequest<Result>, IAuthorizableRequest<Result>
    {
        public Guid Id { get; set; }
        public bool IsHardDelete { get; set; }

        [JsonIgnore]
        public string RequiredPermission => IsHardDelete
            ? Permissions.Accounts.HardDelete
            : Permissions.Accounts.Delete;
    }
}