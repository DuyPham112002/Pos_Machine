using AutoMapper;
using Client_Manager_Services.AuthServices;
using Client_Manager_Services.CategoryServices;
using Client_Manager_Services.SubCategoryService;
using Client_ViewModel.Category;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Mvc;

namespace Client_Manager_MVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAuthService _auth;
        private readonly ICategoryService _category;
        private readonly ISubCategoryService _subcategory;
        public CategoryController(IMapper mapper, IAuthService auth, ICategoryService category, ISubCategoryService subcategory)
        {
            _mapper = mapper;
            _auth = auth;
            _category = category;
            _subcategory = subcategory;
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
                    MainCategoryViewModel model = new MainCategoryViewModel();
                    model.Categories = await _category.GetAllCategoryAsync(token);
                    if (model.Categories != null)
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

        //Create Category
        [HttpPost("[controller]/Create")]
        public async Task<IActionResult> Create(MainCategoryViewModel model)
        {
            var token = HttpContext.Session.GetString("AUTH");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var checkedToken = await _auth.CheckTokenAsync(token);
            if (checkedToken?.RoleName != "Manager") return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Vui lòng kiểm tra lại các thông tin đã nhập";
                model.Categories = await _category.GetAllCategoryAsync(token);
                model.CreateCategory = model.CreateCategory;
                return View("Index", model);
            }

            var create = await _category.CreateAsync(model.CreateCategory, token);
            if (create?.IsSuccess == true)
            {
                return RedirectToAction(nameof(Index), new { success = "Tạo danh mục thành công" });
            }

            return RedirectToAction("Index", new { error = create?.Message ?? "Đã có lỗi hệ thống xảy ra" });
        }

        //Update Active Category
        [HttpPost("[controller]/UpdateActive")]
        public async Task<IActionResult> UpdateActive(string categoryId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    var result = await _category.UpdateActiveAsync(categoryId, token);
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

        [HttpGet("[Controller]/Detail/{categoryId}")]
        public async Task<IActionResult> Detail(string categoryId, string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    DetailCategoryViewModel model = new DetailCategoryViewModel();
                    CategoryViewModel detail = await _category.GetCategoryAsync(categoryId, token);
                    if (detail != null)
                    {
                        model.UpdateCategoryViewModel = _mapper.Map<UpdateCategoryViewModel>(detail);
                        model.SubCategories = await _subcategory.GetAllByCategoryAsync(detail.Id, token);
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
                        return RedirectToAction("Index", "Category", new { error = "Lỗi hệ thống: Không thể tải dữ liệu" });
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        //Update Category
        [HttpPost("[controller]/Update")]
        public async Task<IActionResult> Update(DetailCategoryViewModel model)
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
                        var result = await _category.UpdateAsync(model.UpdateCategoryViewModel, token);
                        if (result?.IsSuccess == true)
                        {
                            return RedirectToAction("Detail", "Category", new { categoryId = model.UpdateCategoryViewModel.Id, success = "Cập nhật danh mục thành công" });
                        }
                        else
                        {
                            return RedirectToAction("Detail", "Category", new { categoryId = model.UpdateCategoryViewModel.Id, error = result?.Message ?? "Lỗi hệ thống: Cập nhật không thành công" });
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Vui lòng kiểm tra lại các thông tin đã nhập";
                        model.SubCategories = await _subcategory.GetAllByCategoryAsync(model.UpdateCategoryViewModel.Id, token);
                        return View("Detail", model);
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }
    }
}
