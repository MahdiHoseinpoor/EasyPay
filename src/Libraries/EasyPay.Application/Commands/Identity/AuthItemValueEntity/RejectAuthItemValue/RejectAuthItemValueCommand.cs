using EasyPay.Application.Common;
using MediatR;
using System.Text.Json.Serialization;

namespace EasyPay.Application.Commands.Identity.AuthItemValueEntity.RejectAuthItemValue
{
    public class RejectAuthItemValueCommand : IRequest<Result>, IAuthorizableRequest<Result>
    {
        [JsonIgnore]
        public long AuthItemValueId { get; set; }
        public string Reason { get; set; }
        public string RequiredPermission => Permissions.AuthItem.Edit;
    }
}