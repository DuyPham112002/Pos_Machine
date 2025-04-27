using Client_POS_Services.AuthServices;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Mvc;

namespace Client_POS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthService _auth;
        public HomeController(IAuthService auth)
        {
            _auth = auth;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Shift");
        }
        [HttpPost]
        public async Task<IActionResult> KeepSessionAlive()
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    // This returns a JSON response with a success message
                    return Ok(new { message = "Session is active" });
                }else return BadRequest();
            }
            else return Unauthorized();
        }
    }
}
