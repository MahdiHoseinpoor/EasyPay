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

namespace EasyPay.Application.Commands.AccountManagement.AccountTypeEntity.DeleteAccountType
{
    public class DeleteAccountTypeCommandHandler : IRequestHandler<DeleteAccountTypeCommand, Result>
    {
        private IAccountTypeRepository _accountTypeRepository;

        public DeleteAccountTypeCommandHandler(IAccountTypeRepository accountTypeRepository)
        {
            _accountTypeRepository = accountTypeRepository;
        }
        public async Task<Result> Handle(DeleteAccountTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _accountTypeRepository.GetByIdAsync(request.Id);
                if (entity == null)
                    return Result.Failure(new NotFoundError());
                await _accountTypeRepository.DeleteAsync(entity, request.IsHardDelete);
                await _accountTypeRepository.SaveChangesAsync();
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
