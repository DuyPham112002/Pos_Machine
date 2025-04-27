using Cosplane_API_DBAccess.Entities;
using Cosplane_API_Service.AuthService;
using Cosplane_API_Service.EmployeeService;
using Cosplane_API_Service.HashService;
using Cosplane_API_Service.RoleService;
using Cosplane_API_Service.TokenService;
using Cosplane_API_ViewModel.Account;
using Cosplane_API_ViewModel.Role;
using Cosplane_API_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cosplane_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IHashService _hash;
        private readonly IAccountService _account;
        private readonly IEmployeeService _emp;
        private readonly IRoleService _role;
        private readonly ITokenService _token;
        private readonly string privateKey = "asndasocdo!@#!@#!asDSAXASDSAX123,.,.,";
        public ManagerController(IHashService hash,IAccountService account
            ,IEmployeeService emp, IRoleService role,ITokenService token)
        {
            _account = account;
            _hash = hash;
            _emp = emp;
            _role = role;
            _token = token;
        
        
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
                RoleViewModel roleInfo = await _role.GetByName("manager");
                //check login
                Account loginResult = await _account.CheckLogin(model,roleInfo.Id);
              
                if (loginResult != null)
                {
                    //generate token
                    string token =  _token.GenerateToken(loginResult.Id, loginResult.Username, roleInfo.Name);
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


        [Authorize(Roles ="admin")]
        [HttpPost]
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
                        if (ModelState.IsValid)
                        {
                            //generate account id
                            string accId = Guid.NewGuid().ToString();
                            model.Account.Id = accId;
                            model.Employee.AccId = accId;
                            //assign manager role id to account
                            RoleViewModel roleInfo = await _role.GetByName("manager");
                            model.Account.RoleId = roleInfo.Id;
                            //hash password
                            model.Account.Password = _hash.SHA256(model.Account.Password + privateKey);
                            //create account 
                            int accResult = await _account.CreateAsync(model.Account, "");
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
    }
}
