using Client_Manager_Services.AuthServices;
using Client_Manager_Services.StaticalService;
using Client_ViewModel;
using Client_ViewModel.Dashboard;
using Client_ViewModel.DashBoard;
using Client_ViewModel.Order;
using Client_ViewModel.Product;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client_Manager_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthService _auth;
        private readonly IStaticalService _statical;
        public HomeController(IAuthService auth, IStaticalService statical)
        {
            _auth = auth;
            _statical = statical;
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
                    StaticalViewModel model = await _statical.GetAsync(token);
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
        [HttpGet]
        public async Task<IActionResult> GetDiseases(int type)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    DiseasesChartViewModel model = await _statical.GetDiseasesAsync(type, token);
                    return Json(model);
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

        [HttpGet]
        public async Task<IActionResult> GetLineses(int type)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    LineChartViewModel model = await _statical.GetLinesAsync(type, token);
                    return Json(model);
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

        [HttpGet]
        public async Task<IActionResult> GetTopOrder(int type)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    IEnumerable<OrderViewModel> model = await _statical.GetTopOrderAsync(type, token);
                    return Json(model);
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

        [HttpGet]
        public async Task<IActionResult> GetTopProduct(int type)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    IEnumerable<ProductViewModel> model = await _statical.GetTopProductAsync(type, token);
                    return Json(model);
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
