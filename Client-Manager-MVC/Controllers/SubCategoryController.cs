using AutoMapper;
using Client_Manager_Services.AuthServices;
using Client_Manager_Services.CategoryServices;
using Client_Manager_Services.ProductService;
using Client_Manager_Services.SubCategoryService;
using Client_ViewModel.SubCategory;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Mvc;

namespace Client_Manager_MVC.Controllers
{
    public class SubCategoryController : Controller
    {
        private readonly IAuthService _auth;
        private readonly IProductService _product;
        private readonly ICategoryService _category;
        private readonly ISubCategoryService _subcategory;
        private readonly IMapper _mapper;
        public SubCategoryController(IAuthService auth, IProductService product, ICategoryService category, ISubCategoryService subcategory, IMapper mapper)
        {
            _auth = auth;
            _product = product;
            _category = category;
            _subcategory = subcategory;
            _mapper = mapper;
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
                    MainSubcategoryViewModel model = new MainSubcategoryViewModel();
                    model.SubCategories = await _subcategory.GetAllAsync(token);
                    model.Categories = await _category.GetAllCategoryAsync(token);
                    if (model != null)
                    {
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
                        return RedirectToAction("Index", "Home", new { error = "Lỗi hệ thống: Không thể tải dữ liệu" });
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

        [HttpGet("[Controller]/Detail/{subCategoryId}")]
        public async Task<IActionResult> Detail(string subCategoryId, string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    DetailSubCategoryViewModel model = new DetailSubCategoryViewModel();
                    SubCategoryViewModel detail = await _subcategory.GetSubCategoryAsync(subCategoryId, token);
                    if (detail != null)
                    {
                        model.UpdateSubCategory = _mapper.Map<UpdateSubCategoryViewModel>(detail);
                        model.Categories = await _category.GetAllCategoryAsync(token);
                        model.Products = await _product.GetAllBySubCategoryAsync(detail.Id, token);
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
                        return RedirectToAction("Index", "SubCategory", new { error = "Lỗi hệ thống: Không thể tải dữ liệu" });
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        //Create SubCategory
        [HttpPost("[controller]/CreateSub")]
        public async Task<IActionResult> CreateSub(MainSubcategoryViewModel model)
        {
            var token = HttpContext.Session.GetString("AUTH");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var checkedToken = await _auth.CheckTokenAsync(token);
            if (checkedToken?.RoleName != "Manager") return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                ViewData["ModelID"] = "#createModel";
                ViewBag.Error = "Vui lòng kiểm tra lại các thông tin đã nhập";
                model.SubCategories = await _subcategory.GetAllAsync(token);
                model.Categories = await _category.GetAllCategoryAsync(token);
                return View("Index", model);
            }

            var create = await _subcategory.CreateAsync(model.CreateSubCategoryViewModel, token);
            if (create?.IsSuccess == true)
            {
                return RedirectToAction(nameof(Index), new { categoryId = model.CreateSubCategoryViewModel.CategoryId, success = "Tạo loại sản phẩm thành công" });
            }

            return RedirectToAction("Index", new { categoryId = model.CreateSubCategoryViewModel.CategoryId, error = create?.Message ?? "Đã có lỗi hệ thống xảy ra" });
        }


        //Update Active SubCategory
        [HttpPost("[controller]/UpdateSubActive")]
        public async Task<IActionResult> UpdateSubActive(string subCategoryId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    var result = await _subcategory.UpdateActiveAsync(subCategoryId, token);
                    if (result?.IsSuccess == true)
                    {
                        return RedirectToAction("Index", new { success = "Cập nhật trạng thái thành công" });
                    }
                    else
                    {
                        return RedirectToAction("Index", new { error = result?.Message ?? "Đã có lỗi hệ thống xảy ra" });
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        //Update SubCategory
        [HttpPost("[controller]/UpdateSub")]
        public async Task<IActionResult> UpdateSub(DetailSubCategoryViewModel model)
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
                        var result = await _subcategory.UpdateAsync(model.UpdateSubCategory, token);
                        if (result?.IsSuccess == true)
                        {
                            return RedirectToAction("Detail", "SubCategory", new { subcategoryId = model.UpdateSubCategory.Id, success = "Cập nhật loại sản phẩm thành công" });
                        }
                        else
                        {
                            return RedirectToAction("Detail", "SubCategory", new { subcategoryId = model.UpdateSubCategory.Id, error = result?.Message ?? "Lỗi hệ thống: Cập nhật không thành công" });
                        }
                    }
                    else
                    {
                        ViewData["ModelID"] = "#updateModal";
                        ViewBag.Error = "Vui lòng kiểm tra lại các thông tin đã nhập";
                        model.Categories = await _category.GetAllCategoryAsync(token);
                        model.Products = await _product.GetAllBySubCategoryAsync(model.UpdateSubCategory.Id, token);
                        return View("Detail", model);
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public async Task<IActionResult> GetSubCategories(string categoryId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    List<SubCategoryViewModel> subCategories = new List<SubCategoryViewModel>();
                    subCategories = await _subcategory.GetAllByCategoryAsync(categoryId, token);
                    return Json(subCategories);
                }
                return Unauthorized();
            }
            return Unauthorized();
        }
    }
}
