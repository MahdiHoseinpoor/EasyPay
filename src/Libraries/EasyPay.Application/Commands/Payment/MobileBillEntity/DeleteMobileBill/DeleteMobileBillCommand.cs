using EasyPay.Application.Common;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace EasyPay.Application.Commands.Payment.MobileBillEntity.DeleteMobileBill
{
    public class DeleteMobileBillCommand : IRequest<Result>, IAuthorizableRequest<Result>
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public string RequiredPermission => Permissions.Bills.Delete;
    }
}