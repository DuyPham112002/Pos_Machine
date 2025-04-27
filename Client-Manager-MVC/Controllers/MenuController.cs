using Client_DBAccess.Entities;
using Client_Manager_Services.AuthServices;
using Client_Manager_Services.MenuService;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Mvc;

namespace Client_Manager_MVC.Controllers
{
    public class MenuController : Controller
    {
        private readonly IAuthService _auth;
        private readonly IMenuService _menu;
        public MenuController(IAuthService auth, IMenuService menu)
        {
            _auth = auth;
            _menu = menu;
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
					var model = await _menu.GetAllAsync(token);
					if (model != null)
					{
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
