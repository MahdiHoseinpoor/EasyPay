using EasyPay.Application.Common;
using EasyPay.Shared.DTOs.Identity;
using MediatR;

namespace EasyPay.Application.Queries.Identity.AuthItemValueEntity.GetSubmissionDetail
{
    public class GetSubmissionDetailQuery : IRequest<Result<SubmissionDetailDto>>, IAuthorizableRequest<Result<SubmissionDetailDto>>
    {
        public long Id { get; set; }
        public string RequiredPermission => Permissions.AuthItem.Edit;
    }
}