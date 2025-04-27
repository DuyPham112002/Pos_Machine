using Client_API_Services.AttributeService;
using Client_API_Services.AttributeSetService;
using Client_API_Services.ImageService;
using Client_API_Services.ImageSetService;
using Client_API_Services.ProductService;
using Client_API_Services.TokenService;
using Client_ViewModel.Image;
using Client_ViewModel.Product;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ITokenService _token;
        private readonly IProductService _product;
        private readonly IImageSetService _imageSet;
        private readonly IImageService _img;
        private readonly IAttributeSetService _attributeSet;
        private readonly IAttributeService _attribute;
        private readonly IHostEnvironment _webHostEnvironment;
        public ProductController(ITokenService token, IProductService product, IImageSetService imageSet, IImageService img, IAttributeSetService attributeSet, IAttributeService attribute, IHostEnvironment webHostEnvironment)
        {
            _token = token;
            _product = product;
            _imageSet = imageSet;
            _img = img;
            _attributeSet = attributeSet;
            _attribute = attribute;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateProductAPIViewModel model)
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
                        var imgsetCreated = await _imageSet.CreateAsync(model.ImgSetId);
                        if (imgsetCreated?.IsSuccess == true)
                        {
                            var attributesetCreated = await _attributeSet.CreateAsync(model.AttributeSetId, checkedToken.AccountId);
                            if (attributesetCreated?.IsSuccess == true)
                            {
                                var result = await _product.CreateAsync(model, checkedToken.AccountId);
                                if (result?.IsSuccess == true)
                                    return Ok();
                                else
                                    return StatusCode(result.HttpResponse.StatusCode, result);
                            }
                            else return StatusCode(imgsetCreated.HttpResponse.StatusCode, attributesetCreated);
                        }
                        else return StatusCode(imgsetCreated.HttpResponse.StatusCode, imgsetCreated);
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
        [HttpPut("Active/{productId}")]
        public async Task<IActionResult> UpdateActive(string productId)
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
                        var result = await _product.UpdateActiveAsync(productId, checkedToken.AccountId);
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

        [Authorize(Roles = "Manager,Employee")]
        [HttpGet("GetById/{productId}")]
        public async Task<IActionResult> Get(string productId)
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
                        var result = await _product.GetAsync(productId, checkedToken.RoleName == "Employee" ? checkedToken.AccountId : string.Empty);
                        if (result?.IsSuccess == true)
                        {
                            var attributeResult = await _attribute.GetByProductAsync(productId);
                            if (attributeResult?.IsSuccess == true)
                                result.Value.Attributes = attributeResult.Value.ToList();
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
        [HttpGet("GetBySubcategory/{subcategoryId}")]
        public async Task<IActionResult> GetBySubcategory(string subcategoryId)
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
                        var result = await _product.GetAllBySubCategoryAsync(subcategoryId, checkedToken.RoleName == "Employee" ? "Active" : null);
                        if (result?.IsSuccess == true)
                        {
                            foreach (var item in result.Value)
                            {
                                if (item.IsRequiredAttribute == 2)
                                {
                                    var attributeResults = await _attribute.GetByProductAsync(item.Id);
                                    if (attributeResults?.IsSuccess == true)
                                        item.Attributes = attributeResults.Value.ToList();
                                }
                            }
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
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateProductViewModel model)
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
                        var result = await _product.UpdateAsync(model, checkedToken.AccountId);
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
                        var result = await _product.GetAllAsync(checkedToken.RoleName == "Employee" ? "Active" : null);
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
                            bool imageSaved = await _img.CreateProductImageAsync(_webHostEnvironment.ContentRootPath, new ImageViewModel()
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
    }
}
