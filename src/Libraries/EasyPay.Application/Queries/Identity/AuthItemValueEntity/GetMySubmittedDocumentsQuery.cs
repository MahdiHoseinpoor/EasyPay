using EasyPay.Shared.DTOs.Identity;
using MediatR;
using System.Collections.Generic;

namespace EasyPay.Application.Queries.Identity.AuthItemValueEntity
{
    public class GetMySubmittedDocumentsQuery : IRequest<Result<List<AuthItemValueDto>>>
    {
    }
}