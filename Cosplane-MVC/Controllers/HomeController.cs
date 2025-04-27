using Cosplane_API_ViewModel.Token;
using Cosplane_MVC_Service.AuthService;
using Microsoft.AspNetCore.Mvc;

namespace Cosplane_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthService _auth;
        public HomeController(IAuthService auth)
        {
            _auth = auth;
        }
        public async Task<IActionResult> Index(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (token != null)
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
