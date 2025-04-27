using Client_API_Services.TokenService;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace POS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ITokenService _token;
        public ImageController(ITokenService token)
        {
            _token = token;
        }

        [Authorize(Roles = "Manager,Employee")]
        [HttpGet("Document/{filename}")]
        public async Task<IActionResult> GetDocument(string filename)
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
                        string root = Directory.GetCurrentDirectory()+ "/wwwroot/Images";
                        string fullPath = Path.Combine(root, filename);
                        if(System.IO.File.Exists(fullPath))
                        {
                            byte[] data = await System.IO.File.ReadAllBytesAsync(fullPath);
                            string base64 = Convert.ToBase64String(data);
                            return Ok(base64);
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
