using Client_API_Services.PaymentService;
using Client_API_Services.TokenService;
using Client_DBAccess.Entities;
using Client_ViewModel.Order;
using Client_ViewModel.Payment;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ITokenService _token;
        private readonly IPaymentService _payment;
        public PaymentController(ITokenService token, IPaymentService payment)
        {
            _token = token;
            _payment = payment;
        }

        [Authorize(Roles = "Employee")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreatePaymentAPIViewModel model)
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
                        var created = await _payment.CreateAsync(model, checkedToken.AccountId);
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

        [Authorize(Roles = "Employee")]
        [HttpGet("Get/{orderId}")]
        public async Task<IActionResult> Get(string orderId)
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
                        var result = await _payment.GetAsync(orderId);
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
