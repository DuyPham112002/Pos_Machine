using Client_Manager_Services.AuthServices;
using Client_Manager_Services.Shift;
using Client_ViewModel.AttendDetailViewModel;
using Client_ViewModel.Shift;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client_Manager_MVC.Controllers
{
    public class ShiftController : Controller
    {
        private readonly IShiftService _shift;
        private readonly IAuthService _auth;

        public ShiftController(IShiftService shift, IAuthService auth)
        {
            _auth = auth;
            _shift = shift;
        }

    
        public async Task<IActionResult> Detail(string shiftId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    ShiftForManagerViewModel result = await _shift.GetAsync(shiftId, token);
                    if (result != null)
                    {
                        return View(result);
                    }
                    else return RedirectToAction(nameof(Index), new { error = "Có lỗi xảy ra không thể xem nhân viên đã làm trong ca" });

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

        public async Task<IActionResult> Index(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    var results = await _shift.GetAll(token);
                    if (results != null)
                    {
                        if (error != null)
                        {
                            ViewBag.Error = error;
                        }
                        if (success != null)
                        {
                            ViewBag.Success = success;
                        }
                        return View(results);
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
    }
}
