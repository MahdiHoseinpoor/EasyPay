using EasyPay.Application.Common;
using MediatR;
using System.Text.Json.Serialization;

namespace EasyPay.Application.Commands.AccountManagement.BankCardEntity.DeleteBankCard
{
    public class DeleteBankCardCommand : IRequest<Result>, IAuthorizableRequest<Result>
    {
        public int Id { get; set; }
        public bool IsHardDelete { get; set; }

        [JsonIgnore]
        public string RequiredPermission => IsHardDelete
            ? Permissions.BankCards.HardDelete
            : Permissions.BankCards.Delete;
    }
}