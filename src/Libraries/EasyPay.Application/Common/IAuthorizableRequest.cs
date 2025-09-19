using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Common
{
    public interface IAuthorizableRequest<out TResponse> : IRequest<TResponse>
    {
        /// <summary>
        /// The permission required to execute this request.
        /// </summary>
        string RequiredPermission { get; }
    }
}
