using EasyPay.Domain.Enums.Identity;
using MediatR;
using System;

namespace EasyPay.Application.Commands.Identity.NaturalUserEntity.CreateNaturalUser
{
    public class CompleteNaturalUserProfileCommand : IRequest<Result>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalCode { get; set; }
        public DateTime BirthDate { get; set; }
        public string FatherName { get; set; }
        public GenderType Gender { get; set; }
    }
}