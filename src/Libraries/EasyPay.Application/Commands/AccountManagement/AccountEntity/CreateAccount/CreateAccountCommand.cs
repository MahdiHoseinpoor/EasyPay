using EasyPay.Common;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Domain.Enums.AccountManagement;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.AccountManagement.AccountEntity.CreateAccount
{
    public class CreateAccountCommand : IRequest<Result<Guid>>
    {
        public int AccountTypeId { get; set; }

        public string Title { get; set; }
    }
}
