using Client_API_Services.IncurredService;
using Client_API_Services.ShiftService;
using Client_API_Services.TokenService;
using Client_Services.IncurredService;
using Client_ViewModel.Incurred;
using Client_ViewModel.Shift;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Client_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ShiftController : ControllerBase
	{
		private readonly IShiftService _shift;
		private readonly ITokenService _token;
		
		public ShiftController(IShiftService shift, ITokenService token)
		{
			_shift = shift;
			_token = token;
		}

        [Authorize(Roles = "Manager")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllShift()
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
                        var result = await _shift.GetAllAsync();
                        if (result.IsSuccess)
                        {
                            return Ok(result.Value);
                        }
                        else return StatusCode(result.HttpResponse.StatusCode, result);
                    }
                    return Unauthorized();
                }
                return Unauthorized();
            }
            return BadRequest();
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("GetShiftbyId/{shiftId}")]
        public async Task<IActionResult> GetShiftbyId(string shiftId)
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
                        var result = await _shift.GetShiftByIdAsync(shiftId);
                        if (result.IsSuccess)
                        {
                            return Ok(result.Value);
                        }
                        else return StatusCode(result.HttpResponse.StatusCode, result);
                    }
                    return Unauthorized();
                }
                return Unauthorized();
            }
            return BadRequest();
        }


        [Authorize(Roles = "Employee")]
		[HttpPost("Start")]
		public async Task<IActionResult> Start(CreateShiftViewModel shift)
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
						var result = await _shift.CreateShiftAsync(shift, checkedToken.AccountId);
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
		[HttpPut("End/{shiftId}")]
		public async Task<IActionResult> End(string shiftId)
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
						/*shift.AccId = checkedToken.AccountId;*/
						var result = await _shift.EndShiftAsync(shiftId);
						if (result.IsSuccess)
						{
							return Ok(result);
						}
						else return StatusCode(result.HttpResponse.StatusCode, result);
					}
					return Unauthorized();

				}
				else return Unauthorized();
			}
			else return Unauthorized();
		}

		[Authorize(Roles = "Employee")]
		[HttpGet]
		public async Task<IActionResult> GetShift()
		{
			string header = Request.Headers["Authorization"].ToString();
			if (header != null && header != string.Empty)
			{
				string token = header.Split(" ")[1];
				if (token != null)
				{
					TokenDecodedViewModel checkedToken = await _token.CheckTokenAsync(token);
					if (checkedToken != null && checkedToken.RoleName == "Employee" )
					{
						var result = await _shift.GetShiftAsync(checkedToken.AccountId);
						if (result.IsSuccess)
						{
							return Ok(result.Value);
						}
						else return StatusCode(result.HttpResponse.StatusCode, result);
					}
					return Unauthorized();
				}
				return Unauthorized();
			}
			return BadRequest();
		}

	}
}
