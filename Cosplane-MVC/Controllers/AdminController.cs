using Cosplane_API_ViewModel.Account;
using Cosplane_API_ViewModel.Employee;
using Cosplane_API_ViewModel.Token;
using Cosplane_MVC_Service.AdminService;
using Cosplane_MVC_Service.AuthService;
using Microsoft.AspNetCore.Mvc;

namespace Cosplane_MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _admin;
        private readonly IAuthService _auth;
        public AdminController(IAdminService admin, IAuthService auth)
        {
            _admin = admin;
            _auth = auth;
        }


        [HttpPost("[Controller]/CreateEmployee")]
        public async Task<IActionResult> CreateEmployeeAsync(CreateFullAccountViewModel model)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkToken = await _auth.CheckTokenAsync(token);
                if (checkToken != null)
                {
                    if (ModelState.IsValid)
                    {
                        bool created = await _admin.CreateAsync(model, token);
                        if (created)
                        {
                            return RedirectToAction(nameof(Create), new
                            {
                                success = "Tạo nhân viên mới thành công"
                            });
                        }
                        else
                        {
                            return RedirectToAction(nameof(Create), new
                            {
                                error = "Không thể tạo mới nhân viên, tài khoản đã tồn tại"
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

        [HttpGet("[Controller]/create")]
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

        [HttpPost("[Controller]/UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(UpdateEmployeeViewModel profile)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkToken = await _auth.CheckTokenAsync(token);
                if (checkToken != null && checkToken.RoleName == "admin")
                {
                    if (ModelState.IsValid)
                    {
                        var newProfile = await _admin.UpdateAsync(profile, token);
                        if (newProfile != null)
                        {
                            return RedirectToAction("UpdateProfile", "Admin", new
                            {
                                success = "Cập nhật thông tin thành công"
                            });
                        }
                        else return RedirectToAction("UpdateProfile", "Admin", new
                        {
                            error = "Cập nhật thông tin thất bại"
                        });
                    }
                    else
                    {
                        string message = string.Join(" & ", ModelState.Values
                          .SelectMany(v => v.Errors)
                          .Select(e => e.ErrorMessage));
                        return RedirectToAction("UpdateProfile", "Admin", new { error = message });
                    }
                }
                else return RedirectToAction("Index", "Auth");
            }
            else return RedirectToAction("Index", "Auth");
        }
        [HttpGet("[Controller]/UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(string success, string error)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkToken = await _auth.CheckTokenAsync(token);
                if (checkToken != null && checkToken.RoleName == "admin")
                {
                    EmployeeViewModel profile = await _admin.GetProfileAsync(token);
                    if (profile != null)
                    {
                        if (error != null)
                        {
                            ViewBag.Error = error;
                        }
                        if (success != null)
                        {
                            ViewBag.Success = success;
                        }
                        return View(profile);
                    }
                    else return RedirectToAction("Index", "Auth", new { error = "Lỗi hệ thống: Không thể mở trang thông tin cá nhân" });
                }
                else return RedirectToAction("Index", "Auth");
            }
            else return RedirectToAction("Index", "Auth");
        }

        [HttpPost("[Controller]/update/{accId}")]
        public async Task<IActionResult> Update(UpdateEmployeeViewModel profile)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkToken = await _auth.CheckTokenAsync(token);
                if (checkToken != null && checkToken.RoleName == "admin")
                {
                    if (ModelState.IsValid)
                    {
                        var newProfile = await _admin.UpdateEmployeeAsync(profile, token);
                        if (ModelState.IsValid)
                        {
                            return RedirectToAction("Update", "Admin", new
                            {
                                success = "Cập nhật thông tin thành công"
                            });
                        }
                        else return RedirectToAction("Update", "Admin", new
                        {
                            Error = "Cập nhật thông tin thất bại"
                        });
                    }
                    else
                    {
                        string message = string.Join(" & ", ModelState.Values
                          .SelectMany(v => v.Errors)
                          .Select(e => e.ErrorMessage));
                        return RedirectToAction("Update", "Admin", new { error = message });
                    }
                }
                else return RedirectToAction("Index", "Auth");
            }
            else return RedirectToAction("Index", "Auth");
        }
        [HttpGet("[Controller]/update/{accId}")]
        public async Task<IActionResult> Update(string accId, string success, string error)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkToken = await _auth.CheckTokenAsync(token);
                if (checkToken != null && checkToken.RoleName == "admin")
                {
                    EmployeeViewModel profile = await _admin.GetEmployeeByIdAsync(accId, token);
                    if (profile != null)
                    {
                        if (error != null)
                        {
                            ViewBag.Error = error;
                        }
                        if (success != null)
                        {
                            ViewBag.Success = success;
                        }
                        return View(profile);
                    }
                    else return RedirectToAction("Index", "Auth", new { error = "Opps! Có lỗ xảy ra" });
                }
                else return RedirectToAction("Index", "Auth");
            }
            else return RedirectToAction("Index", "Auth");
        }


        [HttpPost("[Controller]/delete")]
        public async Task<IActionResult> DeleteAync(string accId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    bool result = await _admin.DeleteByAccountByIdAsync(accId, token);
                    if (result)
                    {
                        return RedirectToAction("Index", "Admin", new { success = "Xóa thành công" });
                    }
                    else return RedirectToAction("Index", "Admin", new { error = "Xóa thất bại" });

                }
                else return RedirectToAction("Index", "Auth");
            }
            else return RedirectToAction("Index", "Auth");
        }
        [HttpPost("[Controller]/Activate")]
        public async Task<IActionResult> Activate(string accId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    bool result = await _admin.ActivateAccountByIdAsync(accId, token);
                    if (result)
                    {
                        return RedirectToAction("Index", "Admin", new { success = "Kích hoạt thành công" });
                    }
                    else return RedirectToAction("Index", "Admin", new { error = "Kích hoạt thất bại" });

                }
                else return RedirectToAction("Index", "Auth");
            }
            else return RedirectToAction("Index", "Auth");

        }

        public async Task<IActionResult> Index(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    List<EmployeeViewModel> result = await _admin.GetAllAsync(token);
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
                        return RedirectToAction("Index", "Auth", new { error = "Opps! Có lỗi xảy ra" });
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

    }
}
