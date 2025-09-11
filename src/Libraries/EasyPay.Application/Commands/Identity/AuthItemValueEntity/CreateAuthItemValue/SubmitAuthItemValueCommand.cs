using MediatR;

namespace EasyPay.Application.Commands.Identity.AuthItemValueEntity.CreateAuthItemValue
{
    public class SubmitAuthItemValueCommand : IRequest<Result<long>>
    {
        public int AuthItemId { get; set; }

        public string Value { get; set; }

        public string? ExtraInfo { get; set; }
    }
}