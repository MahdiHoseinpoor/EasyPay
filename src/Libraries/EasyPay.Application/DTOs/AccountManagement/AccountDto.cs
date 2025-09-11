using EasyPay.Domain.Enums.AccountManagement;
using System;

namespace EasyPay.Application.DTOs.AccountManagement
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string AccountNumber { get; set; }
        public string AccountTypeName { get; set; }
        public decimal CurrentBalance { get; set; }
        public AccountStatus Status { get; set; }
        public DateTime OpeningDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
    }
}