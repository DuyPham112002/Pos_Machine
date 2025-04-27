using Cosplane_API_Service.BrandService;
using Cosplane_API_Service.HashService;
using Cosplane_API_Service.RoleService;
using Cosplane_API_Service.TokenService;
using Cosplane_API_ViewModel.Brand;
using Cosplane_API_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cosplane_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        //fields
        private readonly IHashService _hash;
        private readonly IRoleService _role;
        private readonly ITokenService _token;
        private readonly IBrandService _brand;
        private readonly string privateKey = "asndasocdo!@#!@#!asDSAXASDSAX123,.,.,";

        //constructor with params
        public BrandController(IHashService hash, IRoleService role, ITokenService token, IBrandService brand)
        {
            _hash = hash;
            _role = role;
            _token = token;
            _brand = brand;
        }

        //methods
        //create new brand
        [Authorize(Roles = "admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateBrand(CreateBrandAPIViewModel model)
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
                            int accResult = await _brand.CreateBrandAsync(model, checkedToken.AccountId);
                            if (accResult == 200)
                            {
                                return Ok("Tạo thương hiệu mới thành công");
                            }
                            else if (accResult == 500)
                            {
                                return StatusCode(500);
                            }
                            else return BadRequest("Tên thương hiệu đã tồn tại");
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
        [HttpGet()]
        public async Task<IActionResult> GetAll()
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

        // get a brand by brandId
        [Authorize(Roles = "admin")]
        [HttpGet("{brandId}")]
        public async Task<IActionResult> GetBrand(string brandId)
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
                        GetAllBrandAPIViewModel result = await _brand.GetBrandById(brandId);
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
        [HttpPut()]
        public async Task<IActionResult> Update([FromBody] UpdateBrandAPIViewModel model)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && (checkedToken.RoleName == "admin" ||
                        (checkedToken.RoleName == "admin" && checkedToken.AccountId == model.BrandId)))
                    {
                        if (ModelState.IsValid)
                        {
                            bool result = await _brand.UpdateByBrandIdAsync(model);
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
        [HttpDelete("{brandId}")]
        public async Task<IActionResult> Delete(string brandId)
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
                        bool result = await _brand.DeleteByBrandIdAsync(brandId);
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
        [HttpPut("activate/{brandId}")]
        public async Task<IActionResult> Activate(string brandId)
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
                        bool result = await _brand.ActivateByBrandIdAsync(brandId);
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

        // get brandId by brandName
        [AllowAnonymous]
        [HttpGet("ByName/{brandName}")]
        public async Task<IActionResult> GetBrandId(string brandName)
        {
            GetBrandIdByBrandNameAPIViewModel result = await _brand.GetBrandIdByBrandName(brandName);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
