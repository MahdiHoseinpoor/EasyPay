using EasyPay.Infrastructure.Aggregates.AccountManagement;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Services
{
    public class AccountNumberService : IAccountNumberService
    {
        private readonly IAccountRepository _accountRepository;
        private const int MaxRetries = 5;

        public AccountNumberService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<string> GenerateUniqueAccountNumberAsync()
        {
            for (int i = 0; i < MaxRetries; i++)
            {
                string candidateAccountNumber = GenerateCandidateAccountNumber();

                bool isUnique = !await _accountRepository.ExistsAsync(a => a.AccountNumber == candidateAccountNumber);

                if (isUnique)
                {
                    return candidateAccountNumber;
                }
            }

            throw new ApplicationException("Failed to generate a unique account number after multiple attempts.");
        }

        private string GenerateCandidateAccountNumber()
        {
            const string prefix = "627";
            const int numberLength = 7; 

            var randomNumber = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            int value = BitConverter.ToInt32(randomNumber, 0);

  
            var numberPart = Math.Abs(value % 10000000).ToString("D7");

            return $"{prefix}{numberPart}";
        }
    }
}