using Cosplane_API_DBAccess.Entities;
using Cosplane_API_Service.AuthService;
using Cosplane_API_Service.BrandService;
using Cosplane_API_Service.EmployeeService;
using Cosplane_API_Service.HashService;
using Cosplane_API_Service.RoleService;
using Cosplane_API_Service.TokenService;
using Cosplane_API_ViewModel.Account;
using Cosplane_API_ViewModel.Employee;
using Cosplane_API_ViewModel.Role;
using Cosplane_API_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Cosplane_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AdminController : ControllerBase
	{
		private readonly IHashService _hash;
		private readonly IAccountService _account;
		private readonly IEmployeeService _emp;
		private readonly IRoleService _role;
		private readonly ITokenService _token;
		private readonly string privateKey = "asndasocdo!@#!@#!asDSAXASDSAX123,.,.,";
		public AdminController(IHashService hash, IAccountService account
			, IEmployeeService emp, IRoleService role, ITokenService token)
		{
			_account = account;
			_hash = hash;
			_emp = emp;
			_role = role;
			_token = token;


		}

		[Authorize(Roles = "admin")]
		[HttpPost("CheckToken")]
		public async Task<IActionResult> CheckToken()
		{
			string header = Request.Headers["Authorization"].ToString();
			if (!header.IsNullOrEmpty())
			{
				string token = header.Split(" ")[1];
				if (!token.IsNullOrEmpty())
				{
					TokenDecodedViewModel result = await _token.CheckTokenAsync(token);
					if (result != null && result.RoleName == "admin")
					{
						return Ok(result);
					}
					else return Unauthorized();
				}
				else return Unauthorized();
			}
			else return Unauthorized();
		}

		[AllowAnonymous]
		[HttpPost("Login")]
		public async Task<IActionResult> Login(BasicLoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				//hash password

				model.Password = _hash.SHA256(model.Password + privateKey);
				//get manager role id
				RoleViewModel roleInfo = await _role.GetByName("admin");
				//check login
				Account loginResult = await _account.CheckLogin(model, roleInfo.Id);

				if (loginResult != null)
				{
					//generate token
					string token = _token.GenerateToken(loginResult.Id, loginResult.Username, roleInfo.Name);
					if (token != null)
					{
						//save token
						bool saveResult = await _token.SaveToken(token, loginResult.Id);
						if (saveResult)
						{
							return Ok(token);

						}
						else
						{
							return StatusCode(500);
						}
						//return token
					}
					else
					{
						return StatusCode(500);
					}

				}
				else
				{
					return BadRequest("Username or password is not correct");
				}

			}
			else
			{
				string message = string.Join(" | ", ModelState.Values
					  .SelectMany(v => v.Errors)
					  .Select(e => e.ErrorMessage));

				return BadRequest(message);
			}
		}


		[Authorize(Roles = "admin")]
		[HttpPost("Create")]
		public async Task<IActionResult> Create(CreateFullAccountViewModel model)
		{
			//check token
			string header = Request.Headers["Authorization"].ToString();
			if (header != null)
			{
				string token = header.Split(" ")[1];
				if (token != null)
				{
					TokenDecodedViewModel checkedToken = await _token.CheckTokenAsync(token);
					if (checkedToken != null && checkedToken.RoleName == "admin")
					{
						var emailvalidate = await _emp.CheckMailExist(model.Employee.Email);
						if(emailvalidate?.IsSuccess == false)
						
                            return StatusCode(emailvalidate.HttpResponse.StatusCode, emailvalidate);
                        
                           
                        else if (ModelState.IsValid)
						{
							//generate account id
							string accId = Guid.NewGuid().ToString();
							model.Account.Id = accId;
							model.Employee.AccId = accId;
							//assign manager role id to account
							RoleViewModel roleInfo = await _role.GetByName("admin");
							model.Account.RoleId = roleInfo.Id;
							//hash password
							model.Account.Password = _hash.SHA256(model.Account.Password + privateKey);
							//create account 
							int accResult = await _account.CreateAsync(model.Account, checkedToken.AccountId);
							if (accResult == 200)
							{
								//create emp information
								var empResult = await _emp.CreateAsync(model.Employee);
								if (empResult.IsSuccess)
								{
									return Ok("Create account successful");
								}
								else
								{
									return StatusCode(500);
								}
							}
							else if (accResult == 500)
							{
								return StatusCode(500);
							}
							else
							{
								return BadRequest("Username is already exist");
							}


						}
						else
						{

							string message = string.Join(" | ", ModelState.Values
									.SelectMany(v => v.Errors)
									.Select(e => e.ErrorMessage));

							return BadRequest(message);
						}
					}
					else
					{
						return NotFound();
					}

				}
				else
				{
					return NotFound();
				}

			}
			else
			{
				return NotFound();
			}

		}

		[Authorize(Roles = "admin")]
		[HttpGet("Profile")]
		public async Task<IActionResult> GetProfile()
		{
			string header = Request.Headers["Authorization"].ToString();
			if (header != null && header.Length > 0)
			{
				string token = header.Split(" ")[1];
				if (token != null)
				{
					var checkToken = await _token.CheckTokenAsync(token);
					if (checkToken != null && checkToken.RoleName == "admin")
					{
						var result = await _emp.GetByIdAsync(checkToken.AccountId);
						if (result != null)
						{
                            return Ok(result.Value);
						}
						else
						{
							return StatusCode(result.HttpResponse.StatusCode);
						}
						
					}
					else return Unauthorized();
				}
				else return Unauthorized();
			}
			else return Unauthorized();
		}

		[Authorize(Roles = "admin")]
		[HttpGet("Detail/{accId}")]
		public async Task<IActionResult> GetEmployeeByIdAsync(string accId)
		{
			string header = Request.Headers["Authorization"].ToString();
			if (header != null && header.Length > 0)
			{
				string token = header.Split(" ")[1];
				if (token != null)
				{
					var checkToken = await _token.CheckTokenAsync(token);
					if (checkToken != null && checkToken.RoleName == "admin")
					{
						var result = await _emp.GetByIdAsync(accId);
						if (result.IsSuccess)
						{
							return Ok(result.Value);
						}
						else return StatusCode(result.HttpResponse.StatusCode);
                    }
					else return Unauthorized();
				}
				else return Unauthorized();
			}
			else return Unauthorized();
		}
		[Authorize(Roles ="admin")]
		[HttpPut("Update")]
		public async Task<IActionResult> UpdateEmployeeAsync(UpdateEmployeeViewModel model)
		{
			string header = Request.Headers["Authorization"].ToString();
			if (header != null && header.Length > 0)
			{
				string token = header.Split(" ")[1];
				if (token != null)
				{
					var checkToken = await _token.CheckTokenAsync(token);
					if (checkToken != null && checkToken.RoleName == "admin")
					{
						if (ModelState.IsValid)
						{
							
							var result = await _emp.UpdateAsync(model,model.AccId);
							if (result.IsSuccess)
							{
								return Ok();
							}
							else return StatusCode(result.HttpResponse.StatusCode, result);
						}
						else return Unauthorized();
					}
					else return Unauthorized();

				}
				else return Unauthorized();

			}
			else return Unauthorized();
		}

		[Authorize(Roles = "admin")]
		[HttpPost("Logout")]
		public async Task<IActionResult> Logout()
		{
			string header = Request.Headers["Authorization"].ToString();
			if (header != null && header.Length > 0)
			{
				string token = header.Split(" ")[1];
				if (token != null)
				{
					var checkToken = await _token.CheckTokenAsync(token);
					if (checkToken != null && checkToken.RoleName == "admin")
					{
						var logout = await _token.Logout(checkToken.AccountId);
						if (logout != null)
						{
							return Ok();
						}
						else return BadRequest();
					}
					else return Unauthorized();
				}
				return Unauthorized();
			}
			return Unauthorized();
		}

		[Authorize(Roles = "admin")]
		[HttpPut]
		public async Task<IActionResult> UpdateAsync([FromBody] UpdateEmployeeViewModel model)
		{
			string header = Request.Headers["Authorization"].ToString();
			if (header != null && header.Length > 0)
			{
				string token = header.Split(" ")[1];
				if (token != null)
				{
					var checkToken = await _token.CheckTokenAsync(token);
					if (checkToken != null && checkToken.RoleName == "admin")
					{
						if (ModelState.IsValid)
						{
							
							var result = await _emp.UpdateAsync(model,checkToken.AccountId);
							if (result.IsSuccess)
							{
								return Ok();
							}
							else return StatusCode(result.HttpResponse.StatusCode,result);
						}
						else return BadRequest();
					}
					else return Unauthorized();

				}
				else return Unauthorized();

			}
			else return Unauthorized();
		}

		[Authorize(Roles = "admin")]
		[HttpPut("{accId}")]
		public async Task<IActionResult> DeleteAsync(string accId)
		{

			string header = Request.Headers["Authorization"].ToString();
			if (header != null && header.Length > 0)
			{
				string token = header.Split(" ")[1];
				if (token != null)
				{
					var checkToken = await _token.CheckTokenAsync(token);
					if (checkToken != null && checkToken.RoleName == "admin")
					{
						var result = await _account.DeleteAccount(accId);
						if (result)
						{
							return Ok();
						}
						else return BadRequest();
					}
					return Unauthorized();
				}
				return Unauthorized();
			}
			return Unauthorized();
		}

		[Authorize(Roles = "admin")]
		[HttpPut("Activate/{accId}")]
		public async Task<IActionResult> ActiveAccountAsync(string accId)
		{

			string header = Request.Headers["Authorization"].ToString();
			if (header != null && header.Length > 0)
			{
				string token = header.Split(" ")[1];
				if (token != null)
				{
					var checkToken = await _token.CheckTokenAsync(token);
					if (checkToken != null && checkToken.RoleName == "admin")
					{
						var result = await _account.ActiveAccount(accId);
						if (result)
						{
							return Ok();
						}
						else return BadRequest();
					}
					return Unauthorized();
				}
				return Unauthorized();
			}
			return Unauthorized();
		}

		[Authorize(Roles = "admin")]
		[HttpGet("GetAll")]
		public async Task<IActionResult> GetAll()
		{
			string header = Request.Headers["Authorization"].ToString();
			if (header != null && header.Length > 0)
			{
				string token = header.Split(" ")[1];
				if (token != null)
				{
					var checkToken = await _token.CheckTokenAsync(token);
					if (checkToken != null && checkToken.RoleName == "admin")
					{
						var result = await _emp.GetAllAsync();
						if (result != null)
						{
							return Ok(result.Value);
						}
						return NotFound();
					}
					else return Unauthorized();
				}
				else return Unauthorized();
			}
			else return Unauthorized();
		}
	}
}
