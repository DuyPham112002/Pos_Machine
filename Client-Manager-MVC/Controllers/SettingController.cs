using Client_DBAccess.Entities;
using Client_Manager_Services;
using Client_Manager_Services.AuthServices;
using Client_Manager_Services.SettingService;
using Client_ViewModel.Setting;
using Client_ViewModel.Token;
using Cosplane_API_ViewModel.Brand;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client_Manager_MVC.Controllers
{
    public class SettingController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _auth;
        private readonly IBrandService _brand;
        private readonly ISettingService _setting;
        public SettingController(IConfiguration configuration, IAuthService auth, IBrandService brand, ISettingService setting)
        {
            _configuration = configuration;
            _auth = auth;
            _brand = brand;
            _setting = setting;
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
                    string brandName = _configuration.GetSection("Brand").Value;
                    GetBrandIdByBrandNameAPIViewModel brand = await _brand.GetBrandIdAsync(brandName);
                    if (brand != null)
                    {
                        SettingViewModel model = await _setting.GetAsync(brand.BrandId, token);
                        if (error != null)
                        {
                            ViewBag.Error = error;
                        }
                        if (success != null)
                        {
                            ViewBag.Success = success;
                        }
                        if (model != null)
                            return View(model);
                        else
                            return View(new SettingViewModel() { BrandId = brand.BrandId});
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

        [HttpPost("[controller]/Create")]
        public async Task<IActionResult> Create(SettingViewModel model)
        {
            var token = HttpContext.Session.GetString("AUTH");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var checkedToken = await _auth.CheckTokenAsync(token);
            if (checkedToken?.RoleName != "Manager") return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Vui lòng kiểm tra lại các thông tin đã nhập";
                return View("Index", model);
            }

            var create = await _setting.CreateAsync(model, token);
            if (create?.IsSuccess == true)
            {
                return RedirectToAction(nameof(Index), new { success = "Cập nhật cài đặt thành công" });
            }

            ViewBag.Error = create?.Message ?? "Đã có lỗi hệ thống xảy ra";
            return View("Index", model);
        }

        [HttpPost("[controller]/Update")]
        public async Task<IActionResult> Update(SettingViewModel model)
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
                        var result = await _setting.UpdateAsync(model, token);
                        if (result?.IsSuccess == true)
                        {
                            return RedirectToAction("Index", "Setting", new { success = "Cập nhật cài đặt thành công" });
                        }
                        else
                        {
                            return RedirectToAction("Index", "Setting", new { error = result?.Message ?? "Lỗi hệ thống: Cập nhật không thành công" });
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Vui lòng kiểm tra lại các thông tin đã nhập";
                        return View("Index", model);
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }
    }
}
