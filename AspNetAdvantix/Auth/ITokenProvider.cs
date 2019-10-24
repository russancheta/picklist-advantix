using System;
using Microsoft.IdentityModel.Tokens;

namespace AspNetAdvantix
{
    public interface ITokenProvider
    {
        string CreateToken(User user, DateTime expiry);
 
        // TokenValidationParameters is from Microsoft.IdentityModel.Tokens
        TokenValidationParameters GetValidationParameters();
    }
}