using EasyPay.Infrastructure.Aggregates.AccountManagement;
using MediatR;
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
        public Task<Result> Handle(DeleteAccountTypeCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
