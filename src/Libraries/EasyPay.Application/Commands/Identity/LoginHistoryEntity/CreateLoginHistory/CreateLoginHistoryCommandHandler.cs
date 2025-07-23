using AutoMapper;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Infrastructure.Aggregates.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Commands.Identity.LoginHistoryEntity.CreateLoginHistory
{
    public class CreateLoginHistoryCommandHandler : IRequestHandler<CreateLoginHistoryCommand, Result<long>>
    {
        ILoginHistoryRepository _loginHistoryRepository;
        IMapper _mapper;
        public CreateLoginHistoryCommandHandler(ILoginHistoryRepository loginHistoryRepository, IMapper mapper)
        {
            _loginHistoryRepository = loginHistoryRepository;
            _mapper = mapper;
        }
        public async Task<Result<long>> Handle(CreateLoginHistoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<LoginHistory>(request);
                await _loginHistoryRepository.AddAsync(entity);
                await _loginHistoryRepository.SaveChangesAsync();
                return Result<long>.Success(entity.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
