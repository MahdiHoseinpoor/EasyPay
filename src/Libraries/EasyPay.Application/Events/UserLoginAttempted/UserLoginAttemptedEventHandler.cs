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

namespace EasyPay.Application.Events.UserVerifyPasswordAttempted
{
    public class CreateVerifyPasswordHistoryCommandHandler : INotificationHandler<UserVerifyPasswordAttemptedEvent>
    {
        IVerifyPasswordHistoryRepository _VerifyPasswordHistoryRepository;
        IMapper _mapper;
        public CreateVerifyPasswordHistoryCommandHandler(IVerifyPasswordHistoryRepository VerifyPasswordHistoryRepository, IMapper mapper)
        {
            _VerifyPasswordHistoryRepository = VerifyPasswordHistoryRepository;
            _mapper = mapper;
        }
        async Task INotificationHandler<UserVerifyPasswordAttemptedEvent>.Handle(UserVerifyPasswordAttemptedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<VerifyPasswordHistory>(notification);
                await _VerifyPasswordHistoryRepository.AddAsync(entity);
                await _VerifyPasswordHistoryRepository.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
