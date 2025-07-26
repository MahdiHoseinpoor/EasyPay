using EasyPay.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Application.Services
{
    public interface ITokenService
    {
        Task<TokenResult> GenerateToken(ApplicationUser user);
        Task<ClaimsPrincipal> ValidateToken(string token);
    }

    public record TokenResult(string Token, DateTime Expiry);

   
}
