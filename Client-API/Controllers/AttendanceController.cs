using Client_API_Services.AttendDetailService;
using Client_API_Services.AttendService;
using Client_API_Services.ShiftService;
using Client_API_Services.TokenService;
using Client_DBAccess.Entities;
using Client_ViewModel.AttendDetailViewModel;
using Client_ViewModel.AttendVIewModel;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AttendanceController : ControllerBase
	{
		private readonly ITokenService _token;
		private readonly IAttendanceService _attend;
		private readonly IShiftService _shift;
		private readonly IAttendanceDetailService _attendDetail;
		public AttendanceController(ITokenService token, IAttendanceService attend, IShiftService shift, IAttendanceDetailService attendDetail)
		{
			_token = token;
			_attend = attend;
			_shift = shift;
			_attendDetail = attendDetail;	
		}
		[Authorize(Roles = "Manager")]
		[HttpPost("Create")]
		public async Task<IActionResult> Create(AttendanceViewModel attend)
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
						//get shift by date range
						var getShift = await _shift.GetShiftbyDateRange(attend.DateStart,attend.DateEnd);
						if(getShift != null && getShift.IsSuccess)
						{
							string attId = Guid.NewGuid().ToString();
							attend.Id = attId;
                            // create attendance
                            var createAttend = await _attend.CreateAsync(attend, checkedToken.AccountId);
							if(createAttend.IsSuccess)
							{				
								//foreach list shifht 
								foreach(var item in getShift.Value)
								{
									
									AttendanceDetailViewModel Detail = new AttendanceDetailViewModel();
									Detail.AttendId = attId;
									Detail.AccId = item.AccId;
									Detail.Username = item.Username;
									Detail.BeginBalance = item.BeginAmount;
									Detail.EndBalance = item.EndAmount;
									Detail.TimeStart = item.TimeStart;
									Detail.TimeEnd = item.TimeEnd;
									// create attendance details
									var createAttendanceDetail = await _attendDetail.CreateAsync(Detail);
							/*		if (createAttendanceDetail.IsSuccess)
									{
										return StatusCode(createAttendanceDetail.HttpResponse.StatusCode, createAttendanceDetail);
									}*/
								
								}

								return Ok();
							}
							return StatusCode(createAttend.HttpResponse.StatusCode, createAttend);

						}
						// => null -> return false
						return StatusCode(getShift.HttpResponse.StatusCode, getShift);
						
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
		[HttpGet("GetById/{attendId}")]
		public async Task<IActionResult> GetById(string attendId)
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
						var result = await _attend.GetAsync(attendId);
						if (result?.IsSuccess == true)
						{
							return Ok(result.Value);
						}
						return StatusCode(result.HttpResponse.StatusCode);
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
		[HttpGet("GetAllById/{attendId}")]
		public async Task<IActionResult> GetAllById(string attendId)
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
						var result = await _attendDetail.GetAllByIdAsync(attendId);
						if (result?.IsSuccess == true)
						{
							return Ok(result.Value);
						}
						return StatusCode(result.HttpResponse.StatusCode);
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
		[HttpGet("GetAll")]
		public async Task<IActionResult> GetAll()
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
						var result = await _attend.GetAllAsync();
						if (result?.IsSuccess == true)
						{
							return Ok(result.Value);
						}
						return StatusCode(result.HttpResponse.StatusCode);
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
		[HttpDelete("DeleteById/{attendId}")]
		public async Task<IActionResult> Delete(string attendId)
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
						var deleteDetail = await _attendDetail.DeleteDetailAsync(attendId);
						var result = await _attend.DeleteAsync(attendId);
						if ( deleteDetail.IsSuccess == true && result.IsSuccess == true)
						{
							return Ok(result);
						}
						return StatusCode(result.HttpResponse.StatusCode);
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
	}
}
