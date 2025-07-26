using AutoMapper;
using Azure.Core;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Infrastructure.Aggregates.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Events.UserLoginAttempted
{
    public class CreateLoginHistoryCommandHandler : INotificationHandler<UserLoginAttemptedEvent>
    {
        ILoginHistoryRepository _loginHistoryRepository;
        IMapper _mapper;
        public CreateLoginHistoryCommandHandler(ILoginHistoryRepository loginHistoryRepository, IMapper mapper)
        {
            _loginHistoryRepository = loginHistoryRepository;
            _mapper = mapper;
        }
        async Task INotificationHandler<UserLoginAttemptedEvent>.Handle(UserLoginAttemptedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<LoginHistory>(notification);
                await _loginHistoryRepository.AddAsync(entity);
                await _loginHistoryRepository.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
