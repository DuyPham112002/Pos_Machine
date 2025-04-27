
using Cosplane_API_DBAccess.Entities;
using Cosplane_API_DBAccess.UnitOfWork;
using Cosplane_API_ViewModel.Token;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cosplane_API_Service.TokenService
{
    public interface ITokenService
    {
        string GenerateToken(string accountId, string username, string role);
        Task<TokenDecodedViewModel> CheckTokenAsync(string token);
        Task<bool> SaveToken(string tokenValue, string accId);
        Task<int> Logout(string accId);
    }
    public class TokenService:ITokenService
    {
        private readonly IUnitOfWork _uow;
        public TokenService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        //checkToken
        public async Task<TokenDecodedViewModel> CheckTokenAsync(string token)
        {
            var tokenDecoded = DecodeToken(token);
            if (tokenDecoded != null)
            {
                Account validAcount = await _uow.Account.GetFirstOrDefaultAsync(q => q.Id == tokenDecoded.AccountId && q.IsActive);
                if (validAcount != null)
                {
                    Token validToken = await _uow.Token.GetFirstOrDefaultAsync(q => q.AccId == validAcount.Id && q.IsActive && q.Value == token);
                    if (validToken != null)
                    {
                        
                        if(validToken.CreatedDate.AddDays(1) > DateTime.Now)
                        {
                            return tokenDecoded;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                   
                }
                else return null;
            }
            else
            {
                return null;
            }
        }
        public async Task<bool> SaveToken(string tokenValue,string accId)
        {
            try
            {
                //create or update token
                Token token = await _uow.Token.GetFirstOrDefaultAsync(q => q.AccId == accId);
                if (token == null)
                {
                    Token newToken = new Token()
                    {
                        AccId = accId,
                        CreatedDate = DateTime.Now,
                        Id = Guid.NewGuid().ToString(),
                        IsActive = true,
                        Value = tokenValue
                    };
                    await _uow.Token.AddAsync(newToken);
                    await _uow.SaveAsync();
                    return true;
                }
                else
                {
                    token.Value = tokenValue;
                    token.CreatedDate = DateTime.Now;
                    token.IsActive = true;
                    _uow.Token.Update(token);
                    await _uow.SaveAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
            

        }

        public string GenerateToken(string accountId, string username, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("aoxjdopjopcoe1@#!@#!@dijsaojxdo341!@#!@#.,././2132121321@#$#");

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
        private TokenDecodedViewModel DecodeToken(string token)
        {
            TokenDecodedViewModel result = null;
            try
            {

                var key = Encoding.ASCII.GetBytes("aoxjdopjopcoe1@#!@#!@dijsaojxdo341!@#!@#.,././2132121321@#$#");
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
                string accountId = tokenInfo.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                string username = tokenInfo.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
                string roleName = tokenInfo.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;

                result = new TokenDecodedViewModel()
                {
                    AccountId = accountId,
                    Username = username,
                    RoleName = roleName
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

		public async Task<int> Logout(string accId)
		{
			Token token = await _uow.Token.GetFirstOrDefaultAsync(o => o.AccId == accId);

			if (token != null)
			{
				try
				{
					token.IsActive = false;
					token.Value = string.Empty;
					_uow.Token.Update(token);
					await _uow.SaveAsync();
					return 200;
				}
				catch
				{
					return 500;
				}
			}
			else return 500;
		}
	}
}
