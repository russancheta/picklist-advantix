using System;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Security.Principal;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;

namespace AspNetAdvantix
{
    public class RsaJwtTokenProvider : ITokenProvider
    {
        private RsaSecurityKey _key;
        private string _algorithm;
        private string _issuer;
        private string _audience;
 
        public RsaJwtTokenProvider(string issuer, string audience, string keyName)
        {
            var parameters = new CspParameters { KeyContainerName = keyName };
            var provider = new RSACryptoServiceProvider(2048, parameters);
 
            _key = new RsaSecurityKey(provider);
 
            _algorithm = SecurityAlgorithms.RsaSha256Signature;
            _issuer = issuer;
            _audience = audience;
        }
 
        public string CreateToken(User user, DateTime expiry)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
 
            ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(user.UserName, "jwt"));
 
            // TODO: Add whatever claims the user may have...
 
            SecurityToken token = tokenHandler.CreateJwtSecurityToken(new SecurityTokenDescriptor
            {
                Audience = _audience,
                Issuer = _issuer,
                SigningCredentials = new SigningCredentials(_key, _algorithm),
                Expires = expiry.ToUniversalTime(),
                Subject = identity
            });
 
            return tokenHandler.WriteToken(token);
        }
 
        public TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters
            {
                IssuerSigningKey = _key,
                ValidAudience = _audience,
                ValidIssuer = _issuer,
                ValidateLifetime = true,
                RequireExpirationTime = false,
                ClockSkew = TimeSpan.Zero // Identity and resource servers are the same.
            };
        }
    }
}