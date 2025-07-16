using EasyPay.Common;
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

    }
}
