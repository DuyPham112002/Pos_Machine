using AutoMapper;
using Client_POS_Services.AuthServices;
using Client_Services.AttributeService;
using Client_Services.CategoryServices;
using Client_Services.ProductServices;
using Client_Services.SubCategoryServices;
using Client_ViewModel.Attribute;
using Client_ViewModel.Product;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Mvc;

namespace Client_POS.Controllers
{
    public class ProductController : Controller
    {
        private readonly IAuthService _auth;
        private readonly IProductService _product;
        private readonly ICategoryService _category;
        private readonly ISubCategoryService _subcategory;
        private readonly IAttributeService _attribute;
        private readonly IMapper _mapper;
        public ProductController(IAuthService auth, IProductService product, ICategoryService category, ISubCategoryService subCategory, IAttributeService attribute, IMapper mapper)
        {
            _auth = auth;
            _product = product;
            _category = category;
            _subcategory = subCategory;
            _attribute = attribute;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    MainProductViewModel model = new MainProductViewModel();
                    model.Categories = await _category.GetAllCategoryAsync(token);
                    if (error != null)
                    {
                        ViewBag.Error = error;
                    }
                    if (success != null)
                    {
                        ViewBag.Success = success;
                    }
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Login", "Auth", new { error = "Vui Lòng Đăng Nhập" });
                }
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        [HttpGet("[Controller]/Detail/{productId}")]
        public async Task<IActionResult> Detail(string productId, string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    ProductDetailViewModel model = new ProductDetailViewModel();
                    model.ProductViewModel = await _product.GetProductAsync(productId, token);
                    if (model.ProductViewModel != null)
                    {
                        model.Categories = await _category.GetAllCategoryAsync(token);
                        model.SubCategories = await _subcategory.GetAllByCategoryAsync(model.ProductViewModel.CategoryId, token);
                        if (model.ProductViewModel.IsRequiredAttribute == 2)
                            model.Attributes = await _attribute.GetByProductAsync(productId, token);
                        if (error != null)
                        {
                            ViewBag.Error = error;
                        }
                        if (success != null)
                        {
                            ViewBag.Success = success;
                        }
                        return View(model);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Product", new { error = "Lỗi hệ thống: Không thể tải dữ liệu" });
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Auth");
                }
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }


        [HttpGet("[controller]/document/{filename}")]
        public async Task<IActionResult> GetDocument(string filename)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    byte[] documentData = await _product.GetDocument(filename, token);
                    if (documentData != null)
                    {
                        return File(documentData, "image/jpeg");
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }


        [HttpGet]
        public async Task<IActionResult> GetProducts(string subcategory)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    var products = await _product.GetAllBySubCategoryAsync(subcategory, token);
                    return Json(products);
                }
                return Unauthorized();
            }
            return Unauthorized();
        }

        [HttpGet]
        public async Task<IActionResult> GetProductById(string productId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    var product = await _product.GetProductAsync(productId, token);
                    return Json(product);
                }
                return Unauthorized();
            }
            return Unauthorized();
        }
    }
}
