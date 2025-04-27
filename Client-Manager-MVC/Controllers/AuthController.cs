using Client_Manager_Services.AuthServices;
using Client_ViewModel.Auth;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client_Manager_MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;
        //constructor with params
        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        public async Task<IActionResult> Login(string error)
        {
            if (error != null)
            {
                ViewBag.Error = error;
            }

            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(CheckLoginViewModel model)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (string.IsNullOrEmpty(token))
            {
                if (ModelState.IsValid)
                {
                    TokenResultViewModel result = await _auth.Login(model);
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        session.SetString("USERNAME", model.Username);
                        session.SetString("AUTH", result.Token);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Login", new { error = "Tên đăng nhập hoặc mật khẩu không chính xác" });
                    }
                }
                else return View("Login", model);
            }
            else
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    session.Remove("AUTH");
                    return RedirectToAction("Login");
                }
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
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    var logout = await _auth.Logout(token);
                    if (logout == 200)
                    {
                        session.Remove("AUTH");
                        return RedirectToAction("Login", "Auth");
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index), new { error = "Lỗi Hệ Thống!" });
                    }

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

        public async Task<IActionResult> Forgot(string success, string error)
        {
            if (success != null)
            {
                ViewBag.Success = success;
            }
            if (error != null)
            {
                ViewBag.Error = error;
            }
            return View();
        }
    }
}
