using Cosplane_API_ViewModel.Brand;
using Cosplane_API_ViewModel.Token;
using Cosplane_MVC_Service.AuthService;
using Cosplane_MVC_Service.BrandService;
using Microsoft.AspNetCore.Mvc;

namespace Cosplane_MVC.Controllers
{
    public class BrandController : Controller
    {
        //fields
        private readonly IAuthService _auth;
        private readonly IBrandService _brand;

        //constructor with params
        public BrandController(IAuthService auth, IBrandService brand)
        {
            _auth = auth;
            _brand = brand;
        }

        //methods
        // function to show all brands
        [HttpGet("[controller]")]
        public async Task<IActionResult> Index(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    List<GetAllBrandAPIViewModel> result = await _brand.GetAllAsync(token);
                    if (result != null && result.Count > 0)
                    {
                        if (error != null)
                        {
                            ViewBag.Error = error;
                        }
                        if (success != null)
                        {
                            ViewBag.Success = success;
                        }
                        return View(result);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { error = "Không tìm thấy bất kỳ thương hiệu nào" });
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Auth");
                }
            }
            else
            {
                return RedirectToAction("Index", "Auth");
            }
        }

        //Funtion to process create service
        [HttpPost("[controller]/Create")]
        public async Task<IActionResult> Create(CreateBrandAPIViewModel model)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    if (ModelState.IsValid)
                    {
                        bool created = await _brand.CreateAsync(model, token);
                        if (created)
                        {
                            return RedirectToAction(nameof(Create), new
                            {
                                success = "Tạo thương hiệu mới thành công"
                            });
                        }
                        else
                        {
                            return RedirectToAction(nameof(Create), new
                            {
                                error = "Không thể tạo thương hiệu mới"
                            });
                        }
                    }
                    else
                    {
                        string message = string.Join(" & ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                        return RedirectToAction(nameof(Create), new { error = message });
                    }
                }
                return RedirectToAction("Login", "Auth");
            }
            return RedirectToAction("Login", "Auth");
        }

        //function to show the function create to view
        [HttpGet("[controller]/Create")]
        public async Task<IActionResult> Create(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null)
                {
                    if (error != null)
                    {
                        ViewBag.Error = error;
                    }
                    if (success != null)
                    {
                        ViewBag.Success = success;
                    }
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Auth");
                }

            }
            else
            {
                return RedirectToAction("Index", "Auth");
            }
        }

        //function to process update a brand
        [HttpPost("[controller]/Update/{brandId}")]
        public async Task<IActionResult> Update(UpdateBrandAPIViewModel model)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    bool result = await _brand.UpdateAsync(model, token);
                    if (result)
                    {
                        return RedirectToAction("Update", "Brand", new { success = "Cập nhật thông tin thương hiệu thành công" });

                    }
                    return RedirectToAction("Update", "Brand", new { error = "Cập nhật thông tin thương hiệu gặp lỗi" });

                }
                else
                {
                    return RedirectToAction("Index", "Auth");
                }

            }
            else
            {
                return RedirectToAction("Index", "Auth");
            }
        }

        //function to show the update function
        [HttpGet("[controller]/Update/{brandId}")]
        public async Task<IActionResult> Update(string brandId, string success, string error)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    GetAllBrandAPIViewModel result = await _brand.GetBrandAsync(token, brandId);
                    if (result != null)
                    {

                        if (error != null)
                        {
                            ViewBag.Error = error;
                        }
                        if (success != null)
                        {
                            ViewBag.Success = success;
                        }
                        return View(result);
                    }
                    else
                    {
                        return RedirectToAction("Update", "Brand", new { error = "Cập nhật thông tin thương hiệu gặp lỗi" });
                    }
                }
                else return RedirectToAction("Index", "Auth");
            }
            else return RedirectToAction("Index", "Auth");
        }

        //function to process the function of delete a brand
        [HttpGet("[controller]/Delete/{brandId}")]
        public async Task<IActionResult> Delete(string brandId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    bool result = await _brand.DeleteAsync(token, brandId);
                    if (result)
                    {
                        return RedirectToAction("Index", "Brand", new { success = "Xóa thông tin thương hiệu thành công" });
                    }
                    return RedirectToAction("Index", "Brand", new { error = "Xóa thông tin thương hiệu gặp lỗi" });
                }
                else
                {
                    return RedirectToAction("Index", "Auth");
                }
            }
            else
            {
                return RedirectToAction("Index", "Auth");
            }
        }

        //function to process the function of activate a brand
        [HttpGet("[controller]/Activate/{brandId}")]
        public async Task<IActionResult> Activate(string brandId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    bool result = await _brand.ActivateAsync(token, brandId);
                    if (result)
                    {
                        return RedirectToAction("Index", "Brand", new { success = "Kích hoạt lại thông tin thương hiệu thành công" });
                    }
                    return RedirectToAction("Index", "Brand", new { error = "Kích hoạt thông tin thương hiệu gặp lỗi" });
                }
                else
                {
                    return RedirectToAction("Index", "Auth");
                }
            }
            else
            {
                return RedirectToAction("Index", "Auth");
            }
        }

        //function to show an detail brand
        [HttpGet("[controller]/Detail/{brandId}")]
        public async Task<IActionResult> Detail(string brandId, string success, string error)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    GetAllBrandAPIViewModel result = await _brand.GetBrandAsync(token, brandId);
                    if (result != null)
                    {
                        if (error != null)
                        {
                            ViewBag.Error = error;
                        }
                        if (success != null)
                        {
                            ViewBag.Success = success;
                        }
                        return View(result);
                    }
                    else
                    {
                        return RedirectToAction("Detail", "Brand", new { error = "Không có thông tin của thương hiệu này" });
                    }
                }
                else return RedirectToAction("Index", "Auth");
            }
            else return RedirectToAction("Index", "Auth");
        }
    }
}
