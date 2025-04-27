using Client_API_Services.ImageService;
using Client_API_Services.ImageSetService;
using Client_API_Services.OrderActivityLogService;
using Client_API_Services.OrderDetailService;
using Client_API_Services.OrderService;
using Client_API_Services.TokenService;
using Client_DBAccess.Entities;
using Client_ViewModel.Order;
using Client_ViewModel.OrderDetail;
using Client_ViewModel.Product;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ITokenService _token;
        private readonly IOrderService _order;
        private readonly IOrderDetailService _detail;
        private readonly IOrderActivityLogService _log;
        public OrderController(ITokenService token, IOrderService order, IOrderDetailService detail, IOrderActivityLogService log)
        {
            _token = token;
            _order = order;
            _detail = detail;
            _log = log;
        }



        [Authorize(Roles = "Employee")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateOrderViewModel model)
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
                        var created = await _order.CreateAsync(model.Order, checkedToken.AccountId);
                        if (created?.IsSuccess == true)
                        {
                            var result = await _detail.CreateAsync(model.OrderDetails, model.Order.Id, checkedToken.AccountId);
                            if (result?.IsSuccess == true)
                            {
                                return Ok();
                            }
                            else
                            {
                                return StatusCode(created.HttpResponse.StatusCode, result);
                            }
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
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateOrderViewModel model)
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
                        var result = await _order.UpdateAsync(model, checkedToken.AccountId);
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

        [Authorize(Roles = "Employee")]
        [HttpGet("GetAllByShiftId/{shiftId}")]
        public async Task<IActionResult> GetAllByShiftId(string shiftId)
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
                        var result = await _order.GetAllOrderByShiftIdIsComplete(shiftId);
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
        [Authorize(Roles = "Employee")]
        [HttpGet("All0rderCanceled")]
        public async Task<IActionResult> AllCanceledOrder()
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
                        var result = await _order.GetAllOrderCanceled(checkedToken.RoleName == "Employee" ? checkedToken.AccountId : "Active#Manager$");
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

        [Authorize(Roles = "Employee")]
        [HttpGet("GetAllOrderStatus")]
        public async Task<IActionResult> GetAllOrderStatus()
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
                        var result = await _order.GetAllInCompleteOrderAsync();
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
                        var result = await _order.GetAllAsync();
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

        [Authorize(Roles = "Employee")]
        [HttpGet("AllCompleteOrder")]
        public async Task<IActionResult> AllCompleteOrder()
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    TokenDecodedViewModel checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null &&  checkedToken.RoleName == "Employee")
                    {
                        var result = await _order.GetAllCompleteOrderAsync(checkedToken.RoleName == "Employee" ? checkedToken.AccountId : "Active#Manager$");
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



        [Authorize(Roles = "Manager,Employee")]
        [HttpGet("AllInCompleteOrder")]
        public async Task<IActionResult> AllInCompleteOrder()
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
                        var result = await _order.GetAllInCompleteOrderAsync();
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

        //For Create Payment
        [Authorize(Roles = "Manager,Employee")]
        [HttpGet("GetById/{orderId}")]
        public async Task<IActionResult> Get(string orderId)
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
                        var result = await _order.GetAsync(orderId, checkedToken.RoleName == "Employee" ? checkedToken.AccountId : string.Empty);
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
        //For Order Detail
        [Authorize(Roles = "Manager,Employee")]
        [HttpGet("GetDetailById/{orderId}")]
        public async Task<IActionResult> GetDetail(string orderId)
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
                        var order = new DetailOrderViewModel();
                        var result = await _order.GetAsync(orderId, checkedToken.RoleName == "Employee" ? checkedToken.AccountId : string.Empty);
                        if (result?.IsSuccess == true)
                        {
                            order.Order = result.Value;
                            var details = await _detail.GetDetailByOrderAsync(orderId);
                            if (details?.IsSuccess == true)
                            {
                                order.Orders = details.Value;
                                if (checkedToken.RoleName == "Manager")
                                    order.Logs = await _log.GetActivityLogAsync(orderId);
                                return Ok(order);
                            }
                            return StatusCode(details.HttpResponse.StatusCode);
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
        //For Order Update
        [Authorize(Roles = "Employee")]
        [HttpGet("GetUpdateById/{orderId}")]
        public async Task<IActionResult> GetUpdate(string orderId)
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
                        var result = await _order.GetUpdateAsync(orderId);
                        if (result?.IsSuccess == true)
                        {
                            var details = await _detail.GetUpdateByOrderAsync(orderId);
                            if (details?.IsSuccess == true)
                            {
                                result.Value.OrderDetails = details.Value.ToList();
                                return Ok(result.Value);
                            }
                            return StatusCode(details.HttpResponse.StatusCode);
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


        [Authorize(Roles = "Employee")]
        [HttpPut("UnActived")]
        public async Task<IActionResult> UpdateUnActived(CancelOrderViewModel model)
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
                        var result = await _order.UpdateActiveAsync(model, checkedToken.AccountId);
                        if (result?.IsSuccess == true)
                        {
                            return Ok();
                        }
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
    }
}
