using MediatR;
using System;
using EasyPay.Shared.DTOs.Report;
namespace EasyPay.Application.Queries.Report.TransactionEntity
{
    public class GetMyTransactionHistoryQuery : IRequest<Result<IPagedList<TransactionDto>>>
    {
        public Guid AccountId { get; set; }
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
    }
}