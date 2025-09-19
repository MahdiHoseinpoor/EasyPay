using AutoMapper;
using EasyPay.Common.Errors;
using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.BankCardEntity.DeleteBankCard
{
    public class DeleteBankCardCommandHandler : IRequestHandler<DeleteBankCardCommand, Result>
    {
        private IBankCardRepository _bankCardRepository;
        private IMapper _mapper;
        public DeleteBankCardCommandHandler(IBankCardRepository bankCardRepository, IMapper mapper)
        {
            _bankCardRepository = bankCardRepository;
            _mapper = mapper;
        }

        public async Task<Result> Handle(DeleteBankCardCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _bankCardRepository.GetByIdAsync(request.Id);
                if (entity == null)
                    return Result.Failure(new NotFoundError());
                await _bankCardRepository.DeleteAsync(entity, request.IsHardDelete);
                await _bankCardRepository.SaveChangesAsync();
                return Result.Success();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
            {
                // Error number 547 = FK constraint violation
                return Result.Failure(new DbUpdateError("Can not Delete this AccountType, because it has many realative Account"));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
