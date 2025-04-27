using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Token;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Services.TokenService
{
    public interface ITokenService
    {
        string GenerateToken(string accountId, string username, string role);
        TokenDecodedViewModel DecodeToken(string token);
        Task<TokenDecodedViewModel> CheckTokenAsync(string token);
        Task<bool> SaveToken(string token, string accId);
        Task<int> DeleteToken(string accId);
    }
    public class TokenService : ITokenService
    {

        private readonly IUnitOfWork _uow;

        public TokenService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        //generate token
        public string GenerateToken(string accountId, string username, string role)
        {
            //Check token exist
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("a0.sPqiZ2;}6KE8tPCM]xW:O0#dM)ZI-14hA4da!3;1{I@OEi+[DOe9717iLEa;:ieITP9BU.:F4CR+,1t4)DH/H2IhiykM=WuQ,Hh\"Tx8.ER!,cbS@36[9W\\I?Uu1U'");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, accountId),
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        //decode token
        public TokenDecodedViewModel DecodeToken(string token)
        {
            TokenDecodedViewModel result = null;
            try
            {

                var key = Encoding.ASCII.GetBytes("a0.sPqiZ2;}6KE8tPCM]xW:O0#dM)ZI-14hA4da!3;1{I@OEi+[DOe9717iLEa;:ieITP9BU.:F4CR+,1t4)DH/H2IhiykM=WuQ,Hh\"Tx8.ER!,cbS@36[9W\\I?Uu1U'");
                var handler = new JwtSecurityTokenHandler();
                var validations = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
                var tokenInfo = handler.ValidateToken(token, validations, out var securityToken);

                result = new TokenDecodedViewModel()
                {
                    AccountId = tokenInfo.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value,
                    Username = tokenInfo.Claims.First(claim => claim.Type == ClaimTypes.Name).Value,
                    RoleName = tokenInfo.Claims.First(claim => claim.Type == ClaimTypes.Role).Value
                };
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        //checkToken
        public async Task<TokenDecodedViewModel> CheckTokenAsync(string token)
        {
            var tokenDecoded = DecodeToken(token);
            if (tokenDecoded != null)
            {
                Token validToken = await _uow.Token.GetFirstOrDefaultAsync(q => q.AccId == tokenDecoded.AccountId && q.IsActive, "Acc");
                if (validToken != null && validToken.Acc.IsActive)
                {
                    return tokenDecoded;
                }
                else return null;
            }
            else
            {
                return null;
            }
        }


        //create token and save token after success login
        public async Task<bool> SaveToken(string token, string accId)
        {
            try
            {
                var current = await _uow.Token.GetFirstOrDefaultAsync(q => q.AccId == accId);
                if (current == null)
                {
                    //create new
                    Token newToken = new Token()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Value = token,
                        AccId = accId,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                    };
                    await _uow.Token.AddAsync(newToken);
                }
                else
                {
                    //update
                    current.Value = token;
                    current.AccId = accId;
                    current.IsActive = true;
                    current.CreatedDate = DateTime.Now;
                }
                await _uow.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<int> DeleteToken(string accId)
        {
            try
            {
                Token current = await _uow.Token.GetFirstOrDefaultAsync(q => q.AccId == accId);
                if (current != null)
                {
                    current.Value = "";
                    current.IsActive = false;
                    _uow.Token.Update(current);
                    await _uow.SaveAsync();
                    return 200;
                }
                return 404;
            }
            catch (Exception ex)
            {
                return 500;
            }
        }
    }
}
