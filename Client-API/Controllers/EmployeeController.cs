using Client_API_Services.AccountService;
using Client_API_Services.EmployeeService;
using Client_API_Services.HashService;
using Client_API_Services.ImageService;
using Client_API_Services.ImageSetService;
using Client_API_Services.RoleService;
using Client_API_Services.TokenService;
using Client_DBAccess.Entities;
using Client_ViewModel.Account;
using Client_ViewModel.Auth;
using Client_ViewModel.Employee;
using Client_ViewModel.Image;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly string SECRET = "{POSI@r([xkE@Z/kP7-I({d=Z;D19M09Y24TOsW4hiZu==pv*zGEs(-D19M10Y24HQyB3y0lX/'EmBer:@}";
        private readonly IHostEnvironment _webHostEnvironment;
        private readonly IAccountService _account;
        private readonly IEmployeeService _employee;
        private readonly ITokenService _token;
        private readonly IHashService _hash;
        private readonly IRoleService _role;
        private readonly IImageSetService _imgSet;
        private readonly IImageService _image;
        public EmployeeController(IHostEnvironment webHostEnvironment, IAccountService acc, IEmployeeService employee, IRoleService role, ITokenService token, IHashService hash, IImageSetService imgSet, IImageService image)
        {
            _webHostEnvironment = webHostEnvironment;
            _account = acc;
            _employee = employee;
            _role = role;
            _token = token;
            _hash = hash;
            _imgSet = imgSet;
            _image = image;
        }

        [Authorize(Roles = "Employee")]
        [HttpPost("CheckToken")]
        public async Task<IActionResult> CheckToken()
        {
            string header = Request.Headers["Authorization"].ToString();
            if (!header.IsNullOrEmpty())
            {
                string token = header.Split(" ")[1];
                if (!token.IsNullOrEmpty())
                {
                    TokenDecodedViewModel result = await _token.CheckTokenAsync(token);
                    if (result != null && result.RoleName == "Employee")
                    {
                        return Ok(result);
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(CheckLoginViewModel loginInfo)
        {
            Role roleCheck = await _role.GetByNameAsync("Employee");
            Account account = await _account.CheckLoginAsync(loginInfo.Username,
                   _hash.SHA256(loginInfo.Password + SECRET + "P0ScOmSPbanePR0JEc7bY8inhD@Id18@Id18"), roleCheck.Id);
            if (account == null)
            {
                return NotFound();
            }
            else
            {
                if (account.RoleId == roleCheck.Id)
                {
                    string token = _token.GenerateToken(account.Id, account.Username, roleCheck.Name);
                    if (!string.IsNullOrEmpty(token))
                    {
                        bool save = await _token.SaveToken(token, account.Id);
                        // Return success response with the token
                        if (save)
                            return Ok(new TokenResultViewModel()
                            {
                                Token = token
                            });
                        else return BadRequest();
                    }
                    else return BadRequest();
                }
                else return BadRequest();
            }
        }

        [Authorize(Roles = "Employee")]
        [HttpPut("Logout")]
        public async Task<IActionResult> Logout()
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && checkedToken.RoleName == "Employee")
                    {
                        int logout = await _token.DeleteToken(checkedToken.AccountId);
                        if (logout == 200)
                        {
                            return Ok();
                        }
                        else return StatusCode(logout);
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeViewModel createInfor)
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
                        var emailValidate = await _employee.CheckMailExistAsync(createInfor.Email);
                        if (emailValidate?.IsSuccess == false)
                            return StatusCode(emailValidate.HttpResponse.StatusCode, emailValidate);
                        Role role = await _role.GetByNameAsync("Employee");
                        if (role != null)
                        {
                            createInfor.Account.RoleId = role.Id;
                            createInfor.Account.Id = Guid.NewGuid().ToString();
                            createInfor.ImageSetId = Guid.NewGuid().ToString();
                            createInfor.Account.Password = _hash.SHA256(createInfor.Account.Password + SECRET + "P0ScOmSPbanePR0JEc7bY8inhD@Id18@Id18");
                            var accountCreated = await _account.CreateAsync(createInfor.Account, checkedToken.AccountId, createInfor.brandId);
                            if (accountCreated.IsSuccess)
                            {
                                var imgsetCreated = await _imgSet.CreateAsync(createInfor.ImageSetId);
                                if (imgsetCreated.IsSuccess)
                                {
                                    var result = await _employee.CreateAsync(createInfor, checkedToken.AccountId);
                                    if (result?.IsSuccess == true)
                                    {
                                        return Ok();
                                    }
                                    else
                                    {
                                        return StatusCode(result.HttpResponse.StatusCode, result);
                                    }
                                }
                                else return StatusCode(imgsetCreated.HttpResponse.StatusCode, imgsetCreated);
                            }
                            else return StatusCode(accountCreated.HttpResponse.StatusCode, accountCreated);
                        }
                        else return BadRequest();
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
        [HttpGet("Get/{accId}")]
        public async Task<IActionResult> Get(string accId)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && (checkedToken.RoleName == "Manager" || checkedToken.RoleName == "Employee"))
                    {
                        Employee employee = await _employee.CheckEmployeeExistAsync(accId);
                        if (employee != null)
                        {
                            var result = await _employee.GetByAccIdAsync(accId);
                            if (result.IsSuccess)
                            {
                                return Ok(result.Value);
                            }
                            else
                            {
                                return StatusCode(result.HttpResponse.StatusCode);
                            }
                        }
                        return NotFound();
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("GetAll/{accId}")]
        public async Task<IActionResult> GetAll(string accId)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {

                    var checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && checkedToken.RoleName == "Manager")
                    {
                        var result = await _employee.GetAllByAccIdAsync(accId);
                        if (result.IsSuccess)
                        {
                            return Ok(result.Value);
                        }
                        else
                        {
                            return StatusCode(result.HttpResponse.StatusCode);
                        }
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        [Authorize(Roles = "Manager,Employee")]
        [HttpPost("UploadImage")]
        [RequestSizeLimit(2 * 1024 * 1024)]       //unit is bytes => 2Mb
        [RequestFormLimits(MultipartBodyLengthLimit = 2 * 1024 * 1024)]
        public async Task<IActionResult> UploadImage([FromForm] List<IFormFile> files, [FromForm] string imgSetId)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && (checkedToken.RoleName == "Manager" || checkedToken.RoleName == "Employee"))
                    {
                        string[] permittedExtensions = { ".png", ".jpg", ".jpeg" };
                        //check image
                        foreach (IFormFile image in files)
                        {
                            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");
                            //create folder if not exist
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            //change file name
                            var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
                            var fileName = Path.GetRandomFileName();
                            fileName = Path.ChangeExtension(fileName, ext);
                            string fileNameWithPath = Path.Combine(path, fileName);
                            //save img to server
                            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                            {
                                image.CopyTo(stream);
                            }
                            string imageUrl = "Images/" + fileName;
                            //save img metadata to db
                            bool imageSaved = await _image.CreateEmpoyeeImageAsync(_webHostEnvironment.ContentRootPath, new ImageViewModel()
                            {
                                ImgSetId = imgSetId,
                                ImgUrl = imageUrl
                            });

                            if (!imageSaved)
                                return BadRequest();
                        }
                        return Ok();
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("Active/{accId}")]
        public async Task<IActionResult> Update(string accId)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && checkedToken.RoleName == "Manager")
                    {
                        Employee employee = await _employee.CheckEmployeeExistAsync(accId);
                        if (employee != null)
                        {
                            int result = await _employee.UpdateActiveAsync(employee.AccId);
                            if (result == 200)
                            {
                                return Ok(result);
                            }
                            else return StatusCode(result);
                        }
                        else return NotFound();
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        [Authorize(Roles = "Manager,Employee")]
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateEmployeeViewModel model)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && (checkedToken.RoleName == "Manager" || (checkedToken.RoleName == "Employee" && checkedToken.AccountId == model.AccId)))
                    {
                        var result = await _employee.UpdateProfileAsync(model, checkedToken.AccountId);
                        if (result.IsSuccess)
                        {
                            return Ok();
                        }
                        else return StatusCode(result.HttpResponse.StatusCode, result);
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        [Authorize(Roles = "Manager,Employee")]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && (checkedToken.RoleName == "Manager" || checkedToken.RoleName == "Employee"))
                    {
                        var passwordChange = new ChangePasswordViewModel
                        {
                            AccId = model.AccId,
                            OldPassword = _hash.SHA256(model.OldPassword + SECRET + "P0ScOmSPbanePR0JEc7bY8inhD@Id18@Id18"),
                            NewPassword = _hash.SHA256(model.NewPassword + SECRET + "P0ScOmSPbanePR0JEc7bY8inhD@Id18@Id18"),
                            ConfirmPassword = _hash.SHA256(model.ConfirmPassword + SECRET + "P0ScOmSPbanePR0JEc7bY8inhD@Id18@Id18")
                        };
                        var result = await _employee.ChangePasswordAsync(passwordChange, checkedToken.AccountId);
                        if (result?.IsSuccess == true)
                        {
                            return Ok();
                        }
                        else return StatusCode(result.HttpResponse.StatusCode, result);
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }
    }
}
