using Client_API_Services.SubCategoryService;
using Client_API_Services.TokenService;
using Client_DBAccess.Entities;
using Client_ViewModel.Category;
using Client_ViewModel.SubCategory;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly ITokenService _token;
        private readonly ISubCategorySerivce _subcategory;
        public SubCategoryController(ITokenService token, ISubCategorySerivce subcategory)
        {
            _token = token;
            _subcategory = subcategory;
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateSubCategoryViewModel model)
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
                        var result = await _subcategory.CreateAsync(model, checkedToken.AccountId);
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

        [Authorize(Roles = "Manager")]
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateSubCategoryViewModel model)
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
                        var result = await _subcategory.UpdateAsync(model, checkedToken.AccountId);
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

        [Authorize(Roles = "Manager")]
        [HttpPut("Active/{subCategoryId}")]
        public async Task<IActionResult> UpdateActive(string subCategoryId)
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
                        var result = await _subcategory.UpdateActiveAsync(subCategoryId, checkedToken.AccountId);
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

        [Authorize(Roles = "Manager")]
        [HttpGet("GetById/{subCategoryId}")]
        public async Task<IActionResult> Get(string subCategoryId)
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
                        var result = await _subcategory.GetAsync(subCategoryId);
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
        [HttpGet("GetAllByCategory/{categoryId}")]
        public async Task<IActionResult> GetAllByCategory(string categoryId)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    TokenDecodedViewModel checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && checkedToken.RoleName == "Manager" || checkedToken.RoleName == "Employee")
                    {
                        var result = await _subcategory.GetAllByCategoryAsync(categoryId, checkedToken.RoleName == "Employee" ? "Active" : null);
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
                        var result = await _subcategory.GetAllAsync();
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
