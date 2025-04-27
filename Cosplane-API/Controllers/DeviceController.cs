using Cosplane_API_Service.BrandService;
using Cosplane_API_Service.DeviceService;
using Cosplane_API_Service.HashService;
using Cosplane_API_Service.RoleService;
using Cosplane_API_Service.TokenService;
using Cosplane_API_ViewModel.Brand;
using Cosplane_API_ViewModel.Device;
using Cosplane_API_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cosplane_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        //fields
        private readonly IHashService _hash;
        private readonly IRoleService _role;
        private readonly ITokenService _token;
        private readonly IDeviceService _device;
        private readonly IBrandService _brand;

        //constructor with params
        public DeviceController(IHashService hash, IRoleService role, ITokenService token, IDeviceService device, IBrandService brand)
        {
            _device = device;
            _hash = hash;
            _role = role;
            _token = token;
            _brand = brand;
        }

        //methods
        [Authorize(Roles = "admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateDevice(CreateDeviceAPIViewModel model)
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
                            int accResult = await _device.CreateDeviceAsync(model);
                            if (accResult == 200)
                            {
                                return Ok("Tạo thiết bị mới thành công");
                            }
                            else if (accResult == 500)
                            {
                                return StatusCode(500);
                            }
                            else return BadRequest("Tên thiết bị đã tồn tại");
                        }
                        else
                        {
                            string message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                            return BadRequest(message);
                        }
                    }
                    else return NotFound();
                }
                else return NotFound();
            }
            else return NotFound();
        }

        // get all list of brands
        [Authorize(Roles = "admin")]
        [HttpGet("Brand")]
        public async Task<IActionResult> GetAllBrand()
        {
            //get token 
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && checkedToken.RoleName == "admin")
                    {
                        List<GetAllBrandAPIViewModel> result = await _brand.GetAllAsync();
                        if (result != null && result.Count > 0)
                        {
                            return Ok(result);
                        }
                        else return NotFound();
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        // get list of device by brandId
        [Authorize(Roles = "admin")]
        [HttpGet("By/{brandId}")]
        public async Task<IActionResult> GetDevicesByBrandId(string brandId)
        {
            //get token 
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && checkedToken.RoleName == "admin")
                    {
                        List<GetAllDeviceByBrandAPIViewModel> result = await _device.GetDeviceByBrandId(brandId);
                        if (result != null)
                        {
                            return Ok(result);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }
        //Update a brand by brandId
        [Authorize(Roles = "admin")]
        [HttpPut]
        public async Task<IActionResult> Update(UpdateDeviceAPIViewModel model)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && (checkedToken.RoleName == "admin"))
                    {
                        if (ModelState.IsValid)
                        {
                            bool result = await _device.UpdateByDeviceIdAsync(model);
                            if (result)
                            {
                                return Ok();
                            }
                            else return BadRequest();
                        }
                        else
                        {
                            var message = string.Join(" | ", ModelState.Values
                                              .SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage));
                            return BadRequest(message);
                        }

                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        //delete a brand by brandId
        [Authorize(Roles = "admin")]
        [HttpDelete("{deviceId}")]
        public async Task<IActionResult> Delete(string deviceId)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && checkedToken.RoleName == "admin")
                    {
                        bool result = await _device.DeleteByDeviceIdAsync(deviceId);
                        if (result)
                        {
                            return Ok();
                        }
                        else return BadRequest();
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        //activate a brand by brandId
        [Authorize(Roles = "admin")]
        [HttpPut("activate/{deviceId}")]
        public async Task<IActionResult> Activate(string deviceId)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && checkedToken.RoleName == "admin")
                    {
                        bool result = await _device.ActivateByDeviceIdAsync(deviceId);
                        if (result)
                        {
                            return Ok();
                        }
                        else return BadRequest();
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }
        // get a device of device by deviceId
        [Authorize(Roles = "admin")]
        [HttpGet("{deviceId}")]
        public async Task<IActionResult> GetDeviceByDeviceId(string deviceId)
        {
            //get token 
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && checkedToken.RoleName == "admin")
                    {
                        GetDeviceByIdAPIViewModel result = await _device.GetDeviceById(deviceId);
                        if (result != null)
                        {
                            return Ok(result);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }
    }
}

