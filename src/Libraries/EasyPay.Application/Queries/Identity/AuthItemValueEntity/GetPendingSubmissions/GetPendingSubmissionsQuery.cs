using EasyPay.Application.Common;
using EasyPay.Shared.DTOs.Identity;
using MediatR;

namespace EasyPay.Application.Queries.Identity.AuthItemValueEntity.GetPendingSubmissions
{
    public class GetPendingSubmissionsQuery : IRequest<Result<IPagedList<PendingSubmissionDto>>>, IAuthorizableRequest<Result<IPagedList<PendingSubmissionDto>>>
    {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 15;
        public string RequiredPermission => Permissions.AuthItem.Edit;
    }
}