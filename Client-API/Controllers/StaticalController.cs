using Azure.Core;
using Client_API_Services.StaticalService;
using Client_API_Services.SubCategoryService;
using Client_API_Services.TokenService;
using Client_DBAccess.Entities;
using Client_ViewModel.Dashboard;
using Client_ViewModel.DashBoard;
using Client_ViewModel.Order;
using Client_ViewModel.Product;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaticalController : ControllerBase
    {
        private readonly ITokenService _token;
        private readonly IStaticalService _statical;
        public StaticalController(ITokenService token, IStaticalService statical)
        {
            _token = token;
            _statical = statical;
        }
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> Get()
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
                        StaticalViewModel model = new StaticalViewModel();
                        try
                        {
                            model.Statisticals = await _statical.GetNumberStaticAsync();
                            model.lineChart = await _statical.GetParamLineChartAsync(1);
                            model.AppointmentChart = await _statical.GetParamAppointmentChartAsync();
                            model.SurgeryChart = await _statical.GetParamSurgeryChartAsync();
                            model.DiseasesChart = await _statical.GetParamDiseasesChartAsync(1);
                            model.TopOrders = await _statical.GetTopOrderAsync(1);
                            model.TopProducts = await _statical.GetTopProductAsync(1);
                            return Ok(model);
                        }
                        catch
                        {
                            return StatusCode(500);
                        }
                    }
                    return Unauthorized();
                }
                return Unauthorized();
            }
            return BadRequest();
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("GetDiseases/{type}")]
        public async Task<IActionResult> GetDiseases(int type)
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
                        DiseasesChartViewModel diseasesChart = await _statical.GetParamDiseasesChartAsync(type);
                        if (diseasesChart != null)
                        {
                            return Ok(diseasesChart);
                        }
                        else return NotFound();
                    }
                    return Unauthorized();
                }
                return Unauthorized();  
            }
            return BadRequest();
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("GetLines/{type}")]
        public async Task<IActionResult> GetLine(int type)
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
                        LineChartViewModel lineChart = await _statical.GetParamLineChartAsync(type);
                        if (lineChart != null)
                        {
                            return Ok(lineChart);
                        }
                        else return NotFound();
                    }
                    return Unauthorized();
                }
                return Unauthorized();
            }
            return BadRequest();
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("GetTopOrders/{type}")]
        public async Task<IActionResult> TopOrders(int type)
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
                        IEnumerable<OrderViewModel> orders = await _statical.GetTopOrderAsync(type);
                        if (orders != null)
                        {
                            return Ok(orders);
                        }
                        else return NotFound();
                    }
                    return Unauthorized();
                }
                return Unauthorized();
            }
            return BadRequest();
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("GetTopProducts/{type}")]
        public async Task<IActionResult> TopProducts(int type)
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
                        IEnumerable<ProductViewModel> products = await _statical.GetTopProductAsync(type);
                        if (products != null)
                        {
                            return Ok(products);
                        }
                        else return NotFound();
                    }
                    return Unauthorized();
                }
                return Unauthorized();
            }
            return BadRequest();
        }
    }

}
