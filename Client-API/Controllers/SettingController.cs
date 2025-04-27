using Client_API_Services.SettingService;
using Client_API_Services.TokenService;
using Client_ViewModel.Setting;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly ITokenService _token;
        private readonly ISettingService _setting;
        public SettingController(ITokenService token, ISettingService setting)
        {
            _token = token;
            _setting = setting; 
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(SettingViewModel model)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    TokenDecodedViewModel checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && checkedToken.RoleName == "Manager")
                    {
                        var created = await _setting.CreateAsync(model, checkedToken.AccountId);
                        if (created?.IsSuccess == true)
                        {
                            return Ok();
                        }
                        else
                        {
                            return StatusCode(created.HttpResponse.StatusCode, created);
                        }
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("Update")]
        public async Task<IActionResult> Update(SettingViewModel model)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    TokenDecodedViewModel checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && checkedToken.RoleName == "Manager")
                    {
                        var result = await _setting.UpdateAsync(model, checkedToken.AccountId);
                        if (result?.IsSuccess == true)
                            return Ok();
                        else
                            return StatusCode(result.HttpResponse.StatusCode, result);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [Authorize(Roles = "Manager,Employee")]
        [HttpGet("Get/{brandId}")]
        public async Task<IActionResult> Get(string brandId)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && (checkedToken.RoleName == "Manager" || checkedToken.RoleName == "Employee"))
                    {
                        var result = await _setting.GetAsync(brandId);
                        if (result?.IsSuccess == true)
                        {
                            return Ok(result.Value);
                        }
                        else return StatusCode(result.HttpResponse.StatusCode, result);
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }
    }
}
