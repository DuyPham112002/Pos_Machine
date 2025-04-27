using Client_API_Services.CategoryServices;
using Client_API_Services.TokenService;
using Client_DBAccess.Entities;
using Client_ViewModel.Auth;
using Client_ViewModel.Category;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Security.Policy;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ITokenService _token;
        private readonly ICategoryService _category;
        public CategoryController(ITokenService token, ICategoryService category)
        {
            _token = token;
            _category = category;
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
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
                        var result = await _category.CreateAsync(model, checkedToken.AccountId);
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
        public async Task<IActionResult> Update(UpdateCategoryViewModel model)
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
                        var result = await _category.UpdateAsync(model, checkedToken.AccountId);
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
        [HttpPut("Active/{categoryId}")]
        public async Task<IActionResult> UpdateActive(string categoryId)
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
                        var result = await _category.UpdateActiveAsync(categoryId, checkedToken.AccountId);
                        if(result?.IsSuccess == true)
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
        [HttpGet("GetById/{categoryId}")]
        public async Task<IActionResult> Get(string categoryId)
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
                        var result = await _category.GetAsync(categoryId);
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
                    if (checkedToken != null && (checkedToken.RoleName == "Manager" || checkedToken.RoleName == "Employee"))
                    {
                        var result = await _category.GetAllAsync(checkedToken.RoleName == "Employee" ? "Active" : null);
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
