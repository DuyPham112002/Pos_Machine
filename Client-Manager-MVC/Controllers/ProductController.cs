using AutoMapper;
using Client_Manager_MVC.Utilities;
using Client_Manager_Services.AttributeService;
using Client_Manager_Services.AuthServices;
using Client_Manager_Services.CategoryServices;
using Client_Manager_Services.ProductService;
using Client_Manager_Services.SubCategoryService;
using Client_ViewModel.Attribute;
using Client_ViewModel.Category;
using Client_ViewModel.Image;
using Client_ViewModel.Product;
using Client_ViewModel.SubCategory;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Mvc;

namespace Client_Manager_MVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _product;
        private readonly IAuthService _auth;
        private readonly ICategoryService _category;
        private readonly ISubCategoryService _subcategory;
        private readonly IMapper _mapper;
        private readonly IAttributeService _attribute;
        public ProductController(IProductService product, IAuthService auth, ICategoryService category, ISubCategoryService subcategory, IMapper mapper, IAttributeService attribute)
        {
            _product = product;
            _auth = auth;
            _category = category;
            _subcategory = subcategory;
            _mapper = mapper;
            _attribute = attribute;
        }

        public async Task<IActionResult> Index(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    MainProductViewModel model = new MainProductViewModel();
                    var categories = await _category.GetAllCategoryAsync(token);
                    if(categories != null)
                    {
                        model.Categories = categories.Where(p => p.IsActive).ToList();
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
                    return RedirectToAction("Index", "Home", new { error = "Lỗi hệ thống: không thể tải dữ liệu" });
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

        public async Task<IActionResult> Create(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    CreateProductViewModel model = new CreateProductViewModel();
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
                    return RedirectToAction("Login", "Auth");
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
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    ProductDetailViewModel model = new ProductDetailViewModel();
                    model.ProductViewModel = await _product.GetProductAsync(productId, token);
                    if (model.ProductViewModel != null)
                    {
                        model.UpdateProductViewModel = _mapper.Map<UpdateProductViewModel>(model.ProductViewModel);
                        model.Categories = await _category.GetAllCategoryAsync(token);
                        model.SubCategories = await _subcategory.GetAllByCategoryAsync(model.ProductViewModel.CategoryId, token);
                        var attributes = await _attribute.GetByProductAsync(productId, token);
                        if (attributes != null) {
                            model.UpdateAttributeViewModel.AttributeSetId = model.ProductViewModel.AttributeSetId;
                            if (model.UpdateProductViewModel.IsRequiredAttribute != 1)
                                model.UpdateAttributeViewModel.Updates = _mapper.Map<List<UpdateAttributeAPIViewModel>>(attributes);
                            else
                                model.UpdateAttributeViewModel.Updates = new List<UpdateAttributeAPIViewModel>() { new UpdateAttributeAPIViewModel() };

                        }
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

        [HttpPost("[controller]/Create")]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            var token = HttpContext.Session.GetString("AUTH");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var checkedToken = await _auth.CheckTokenAsync(token);
            if (checkedToken?.RoleName != "Manager") return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Vui lòng kiểm tra lại các thông tin đã nhập";
                model.Categories = await _category.GetAllCategoryAsync(token);
                if (!string.IsNullOrEmpty(model.CreateProduct.CategoryId))
                    model.SubCategories = await _subcategory.GetAllByCategoryAsync(model.CreateProduct.CategoryId, token);
                return View("Create", model);
            }

            //check image
            model.Image.ImgSetId = model.CreateProduct.ImgSetId;
            string[] permittedExtensions = { ".png", ".jpg", ".jpeg" };
            string messageError = string.Empty;
            foreach (IFormFile image in model.Image.Images)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    if (memoryStream.Length == 0)
                    {
                        messageError = "File ảnh có thể bị lỗi. Vui lòng kiểm tra lại";
                        break;
                    }
                    if (!FileHelper.IsValidFileExtensionAndSignature(image.FileName, memoryStream, permittedExtensions))
                    {
                        messageError = "Vui lòng upload .jpg, .png, .jpeg";
                        break;
                    }
                }
                if (image.Length > (2 * 1024 * 1024))
                {
                    messageError = "Vui lòng upload ảnh nhỏ hơn 2MB";
                    break;
                }
            }
            if (model.CreateProduct.IsRequiredAttribute != 1 && (model.CreateAttribute.Attributes == null || model.CreateAttribute.Attributes.FirstOrDefault(p => !p.IsDeleted) == null)) {
                messageError = "Vui lòng tạo phân loại sản phẩm";
            }
            if (messageError != string.Empty)
            {
                ViewBag.Error = messageError;
                model.Categories = await _category.GetAllCategoryAsync(token);
                if (!string.IsNullOrEmpty(model.CreateProduct.CategoryId))
                    model.SubCategories = await _subcategory.GetAllByCategoryAsync(model.CreateProduct.CategoryId, token);
                return View("Create", model);
            }
            //Create Product
            var create = await _product.CreateAsync(model.CreateProduct, token);
            if (create?.IsSuccess == true)
            {
                //Update Image
                var upload = await _product.UploadImage(model.Image, token);
                if (!upload)
                {
                    return RedirectToAction("Create", new { error = "Đã có lỗi xảy ra trong quá trình upload ảnh sản phẩm" });
                }
                //Create Attribute
                model.CreateAttribute.AttributeSetId = model.CreateProduct.AttributeSetId;
                if (model.CreateProduct.IsRequiredAttribute == 2)
                {
                    var attributed = await _attribute.CreateAsync(model.CreateAttribute, token);
                    if (attributed?.IsSuccess == false)
                    {
                        return RedirectToAction("Create", new { error = attributed?.Message ?? "Đã có lỗi xảy ra trong quá trình tạo loại sản phẩm" });
                    }
                }
                return RedirectToAction("Create", new { success = "Tạo sản phẩm thành công" });
            }

            return RedirectToAction("Create", new { error = create?.Message ?? "Đã có lỗi hệ thống xảy ra" });
        }

        [HttpPost("[controller]/Update")]
        public async Task<IActionResult> Update(ProductDetailViewModel model)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    string messageError = string.Empty;
                    if (model.UpdateProductViewModel.IsRequiredAttribute != 1 && (model.UpdateAttributeViewModel.Updates == null || model.UpdateAttributeViewModel.Updates.FirstOrDefault(p => !p.IsDeleted) == null))
                    {
                        messageError = "Vui lòng tạo phân loại sản phẩm";
                    }
                    if (ModelState.IsValid && messageError == string.Empty)
                    {   
                        var result = await _product.UpdateAsync(model.UpdateProductViewModel, token);
                        if (result?.IsSuccess == true)
                        {
                            if(model.UpdateProductViewModel.IsRequiredAttribute == 2)
                            {
                                var attributed = await _attribute.UpdateAsync(model.UpdateAttributeViewModel, token);
                                if (attributed?.IsSuccess == false)
                                {
                                    return RedirectToAction("Detail", "Product", new { productId = model.UpdateProductViewModel.Id, error = attributed?.Message ?? "Đã có lỗi xảy ra trong quá trình cập nhật sản phẩm" });
                                }
                            }
                            return RedirectToAction("Detail", "Product", new { productId = model.UpdateProductViewModel.Id, success = "Cập nhật sản phẩm thành công" });
                        }
                        else
                        {
                            return RedirectToAction("Detail", "Product", new { productId = model.UpdateProductViewModel.Id, error = result?.Message ?? "Lỗi hệ thống: Cập nhật không thành công" });
                        }
                    }
                    else
                    {
                        ViewBag.Error = messageError != string.Empty ? messageError : "Vui lòng kiểm tra lại các thông tin đã nhập";
                        model.ProductViewModel = await _product.GetProductAsync(model.UpdateProductViewModel.Id, token);
                        model.Categories = await _category.GetAllCategoryAsync(token);
                        model.SubCategories = await _subcategory.GetAllByCategoryAsync(model.ProductViewModel.CategoryId, token);
                        return View("Detail", model);
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        [HttpPost("[controller]/UpdateActive")]
        public async Task<IActionResult> UpdateActive(string productId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    var result = await _product.UpdateActiveAsync(productId, token);
                    if (result?.IsSuccess == true)
                    {
                        return RedirectToAction("Index", new { index = 0, success = "Cập nhật trạng thái thành công" });
                    }
                    else
                    {
                        return RedirectToAction("Index", new { index = 0, error = result?.Message ?? "Đã có lỗi hệ thống xảy ra" });
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        [HttpGet("[controller]/document/{filename}")]
        public async Task<IActionResult> GetDocument(string filename)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
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

        [HttpPost("[controller]/ChangeImage")]
        public async Task<IActionResult> ChangeImage(ProductDetailViewModel model)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    if (ModelState.IsValid)
                    {
                        string messageError = string.Empty;
                        if (model.Image != null && model.Image.Images != null && model.Image.Images.Count() > 0)
                        {
                            string[] permittedExtensions = { ".png", ".jpg", ".jpeg" };
                            //check image
                            foreach (IFormFile image in model.Image.Images)
                            {

                                using (var memoryStream = new MemoryStream())
                                {
                                    await image.CopyToAsync(memoryStream);
                                    if (memoryStream.Length == 0)
                                    {
                                        messageError = "File ảnh có thể bị lỗi. Vui lòng kiểm tra lại";
                                        break;
                                    }
                                    if (!FileHelper.IsValidFileExtensionAndSignature(image.FileName, memoryStream, permittedExtensions))
                                    {
                                        messageError = "Vui lòng upload .jpg, .png, .jpeg";
                                        break;
                                    }
                                }
                                if (image.Length > (2 * 1024 * 1024))
                                {
                                    messageError = "Vui lòng upload ảnh nhỏ hơn 2MB";
                                    break;
                                }
                            }
                            if (messageError != string.Empty)
                            {
                                var routeValues = new { productId = model.Image.ProductId, error = messageError };
                                return RedirectToAction("Detail", "Product", routeValues);
                            }
                            bool isUploaded = await _product.UploadImage(model.Image, token);
                            if (isUploaded)
                            {
                                var routeValues = new { productId = model.Image.ProductId, success = "Cập nhật hình ảnh thành công" };
                                return RedirectToAction("Detail", "Product", routeValues);
                            }
                            else
                            {
                                var routeValues = new { productId = model.Image.ProductId, error = "Cập nhật hình ảnh không thành công" };
                                return RedirectToAction("Detail", "Product", routeValues);
                            }
                        }
                        else
                        {
                            var routeValues = new { productId = model.Image.ProductId, error = "Vui lòng chọn hình ảnh" };
                            return RedirectToAction("Detail", "Product", routeValues);
                        }
                    }
                    else
                    {
                        var routeValues = new { productId = model.Image.ProductId, error = "Vui lòng kiểm tra lại hình ảnh đã chọn" };
                        return RedirectToAction("Detail", "Product", routeValues);
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(string subcategoryId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    var products = await _product.GetAllBySubCategoryAsync(subcategoryId, token);
                    return Json(products);
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

        //[HttpPost]
        //public async Task<IActionResult> SearchByName(string nameSearch, string subcategoryId)
        //{
        //    ISession session = HttpContext.Session;
        //    string token = session.GetString("AUTH");
        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
        //        if (checkedToken != null && checkedToken.RoleName == "Manager")
        //        {
        //            MainProductViewModel model = new MainProductViewModel();
        //            model.Products = await _product.GetAllBySubCategoryAsync(subcategoryId, token);
        //            model.SearchName = nameSearch;
        //            return View("Index", model);
        //        }
        //        else
        //        {
        //            return Unauthorized();
        //        }
        //    }
        //    else
        //    {
        //        return Unauthorized();
        //    }
        //}
    }
}
