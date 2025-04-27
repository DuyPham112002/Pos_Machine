using Client_API_Services.OrderDetailService;
using Client_API_Services.TokenService;
using Client_DBAccess.Entities;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly ITokenService _token;
        private readonly IOrderDetailService _order;
        public OrderDetailController(ITokenService token, IOrderDetailService order)
        {
            _token = token; 
            _order = order;
        }

        [Authorize(Roles = "Manager,Employee")]
        [HttpGet("GetAllByOrder/{orderId}")]
        public async Task<IActionResult> GetAllByOrder(string orderId)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    TokenDecodedViewModel checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && (checkedToken.RoleName == "Manager" || checkedToken.RoleName == "Employee"))
                    {
                        var result = await _order.GetAllAsync(orderId);
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
    }
}
