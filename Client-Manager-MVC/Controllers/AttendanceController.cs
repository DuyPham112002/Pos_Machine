using Client_DBAccess.Entities;
using Client_Manager_Services.AttendanceServices;
using Client_Manager_Services.AuthServices;
using Client_ViewModel.AttendDetailViewModel;
using Client_ViewModel.AttendVIewModel;
using Client_ViewModel.Incurred;
using Client_ViewModel.Shift;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client_Manager_MVC.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendance;
        private readonly IAuthService _auth;

        public AttendanceController(IAttendanceService attendance, IAuthService auth)
        {
            _auth = auth;
            _attendance = attendance;
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(string attendId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);

                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {

                    var created = await _attendance.DeleteAsync(attendId, token);
                    if (created.IsSuccess)
                    {
                        return RedirectToAction(nameof(Index), new
                        {
                            success = "Xóa thành công"
                        });
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index), new
                        {
                            error = "Không thể xóa ca"
                        });
                    }
                }
                return RedirectToAction("Login", "Auth");
            }
            return RedirectToAction("Login", "Auth");
        }



        /* [HttpGet("Detail")]*/
        [HttpGet("[Controller]/Detail/{attendId}")]
        public async Task<IActionResult> Detail(string attendId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    List<AttendanceDetailViewModel> result = await _attendance.GetAllByIdAsync(attendId, token);
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

        [HttpPost("Create")]
        public async Task<IActionResult> CreateView(AttendanceViewModel model)
        {

            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkToken = await _auth.CheckTokenAsync(token);
                if (checkToken != null && checkToken.RoleName == "Manager")
                {
                    if (ModelState.IsValid)
                    {
                        var created = await _attendance.CreateAsync(model, token);
                        if (created?.IsSuccess == true)
                        {
                            return RedirectToAction(nameof(Index), new
                            {
                                success = "Tạo ca chấm công thành công"
                            });
                        }
                        else
                        {
                            return RedirectToAction(nameof(Create), new
                            {
                                error = created?.Message ?? "Không thể tạo mới ca chấm công, vui lòng thử lại sau"
                            });
                        }
                    }
                    else
                    {
                        string message = string.Join(" & ", ModelState.Values
                          .SelectMany(v => v.Errors)
                          .Select(e => e.ErrorMessage));
                        return RedirectToAction(nameof(Create), new { error = message });
                    }

                }

                return RedirectToAction("Login", "Auth");

            }
            return RedirectToAction("Login", "Auth");
        }

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

        public async Task<IActionResult> Index(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    var results = await _attendance.GetAllAsync(token);
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
