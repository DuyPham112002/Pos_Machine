using Client_API_Services.IncurredService;
using Client_API_Services.TokenService;
using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Incurred;
using Client_ViewModel.Incurred;
using Client_ViewModel.Shift;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncurredController : ControllerBase
    {
        private readonly ITokenService _token;
        private readonly IIcurredService _incurred;
        public IncurredController(ITokenService token, IIcurredService incurred)
        {
            _token = token;
            _incurred = incurred;
        }

        [Authorize(Roles = "Employee")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateIncurredViewModel incurred)
        {

            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    TokenDecodedViewModel checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && checkedToken.RoleName == "Employee")
                    {
                        var result = await _incurred.CreateIncurred(incurred, checkedToken.AccountId);
                        if (result.IsSuccess)
                        {
                            return Ok(result);
                        }
                        else return StatusCode(result.HttpResponse.StatusCode, result);
                    }
                    else return Unauthorized();

                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }


        [Authorize(Roles = "Employee")]
        [HttpGet("{shiftId}")]
        public async Task<IActionResult> GetIncurredById(string shiftid)
        {

            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    TokenDecodedViewModel checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && checkedToken.RoleName == "Employee")
                    {
                        var result = await _incurred.GetIncurredByShifIdAsync(shiftid);
                        if (result.IsSuccess)
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
