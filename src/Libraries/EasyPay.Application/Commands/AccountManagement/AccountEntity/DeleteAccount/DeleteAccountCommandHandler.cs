using EasyPay.Application.Commands.AccountManagement.AccountEntity.DeleteAccount;
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

namespace EasyPay.Application.Commands.AccountManagement.AccountEntity.DeleteAccount
{
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Result>
    {
        private IAccountRepository _accountRepository;

        public DeleteAccountCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<Result> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _accountRepository.GetByIdAsync(request.Id);
                if (entity == null)
                    return Result.Failure(new NotFoundError());
                if (request.IsHardDelete)
                {
                    //TODO: Check User has permissions
                }
                await _accountRepository.DeleteAsync(entity, request.IsHardDelete);
                await _accountRepository.SaveChangesAsync();
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
