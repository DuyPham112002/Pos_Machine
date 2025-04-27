using Cosplane_API_ViewModel.Account;
using Cosplane_API_ViewModel.Token;
using Cosplane_MVC_Service.AuthService;
using Cosplane_MVC_ViewModel.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Cosplane_MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }
        [HttpPost("CheckLogin")]
        public async Task<IActionResult> CheckLogin(BasicLoginViewModel info)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (token != null)
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null)
                {
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        LoginResultViewModel result = await _auth.AdminLoginAsync(info);
                        if (result != null)
                        {
                            if (result.Status)
                            {
                                session.SetString("AUTH", result.Message);
                                return RedirectToAction("Index", "Home", new { success = "Đăng nhập thành công!" });
                            }
                            else
                            {
                                return RedirectToAction(nameof(Login), new { error = result.Message });
                            }
                        }
                        else
                        {
                            return RedirectToAction(nameof(Login), new { error = "Xảy ra lỗi. Vui lòng thử lại" });
                        }
                    }
                    else
                    {
                        return RedirectToAction(nameof(Login), new { error = "Vui lòng nhập tên đăng nhập và mật khẩu" });
                    }

                }

            }
            else
            {
                if (ModelState.IsValid)
                {
                    LoginResultViewModel result = await _auth.AdminLoginAsync(info);
                    if (result != null)
                    {
                        if (result.Status)
                        {
                            session.SetString("AUTH", result.Message);
                            return RedirectToAction("Index", "Home", new { success = "Đăng nhập thành công!" });
                        }
                        else
                        {
                            return RedirectToAction(nameof(Login), new { error = result.Message });
                        }
                    }
                    else
                    {
                        return RedirectToAction(nameof(Login), new { error = "Xảy ra lỗi. Vui lòng thử lại" });
                    }
                }
                else
                {
                    return RedirectToAction(nameof(Login), new { error = "Vui lòng nhập tên đăng nhập và mật khẩu" });
                }
            }

        }

        public async Task<IActionResult> Login(string error)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (token != null)
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null)
                {
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    if (error == null)
                    {
                        ViewBag.Error = error;
                    }
                    return View();
                }

            }
            else
            {
                return View();
            }

        }
        [HttpGet("[Controller]/Logout")]
        public async Task<IActionResult> Logout()
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    bool logout = await _auth.Logout(token);
                    if (logout)
                    {
                        session.Remove("AUTH");
                        return RedirectToAction("Login", "Auth");
                    }
                    return RedirectToAction(nameof(Login), new { error = "Xảy ra lỗi. Vui lòng thử lại" });

                }
                return RedirectToAction(nameof(Login), new { error = "Xảy ra lỗi. Vui lòng thử lại" });
            }
            return RedirectToAction(nameof(Login), new { error = "Xảy ra lỗi. Vui lòng thử lại" });

        }
    }
}
