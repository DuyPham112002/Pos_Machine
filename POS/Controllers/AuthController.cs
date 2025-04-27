using Client_POS_Services;
using Client_POS_Services.AuthServices;
using Client_Services.FingerprintService;
using Client_Services.ShiftServices;
using Client_ViewModel.Auth;
using Client_ViewModel.Fingerprint;
using Client_ViewModel.Token;
using Cosplane_API_ViewModel.Brand;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client_POS.Controllers
{
    public class AuthController : Controller
    {
        //fields
        private readonly IAuthService _auth;
        private readonly IShiftService _shift;
        private readonly IFingerprintService _fingerprintService;
        private readonly IBrandService _brand;
        private readonly IConfiguration _configuration;
        private readonly IGenerateFingerprint _fingerprint;
        //constructor with params
        public AuthController(IAuthService auth, IShiftService shift, IFingerprintService fingerprintService, IBrandService brand,
            IConfiguration configuration, IGenerateFingerprint fingerprint)
        {
            _auth = auth;
            _shift = shift;
            _fingerprintService = fingerprintService;
            _brand = brand;
            _configuration = configuration;
            _fingerprint = fingerprint;
        }
        //methods
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
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    return RedirectToAction("Index", "Shift");
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
        public async Task<IActionResult> SignIn(CheckDeviceViewModel model)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (token == null || string.IsNullOrEmpty(token))
            {
                if (ModelState.IsValid)
                {
                    TokenResultViewModel result = await _auth.Login(model.login);
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        string brandName = _configuration.GetSection("Brand").Value;
                        GetBrandIdByBrandNameAPIViewModel brandId = await _brand.GetBrandIdAsync(brandName);
                        if (brandId != null)
                        {
                            // Kiểm tra nếu fingerprint chưa được khởi tạo
                            if (model.fingerprint == null)
                            {
                                model.fingerprint = new CheckFingerprintViewModel();
                            }
                            // Gán BrandId cho fingerprint
                            model.fingerprint.BrandId = brandId.BrandId;
                            // Lấy thông tin từ middleware
                            string userAgent = HttpContext.Items["UserAgent"]?.ToString();
                            string acceptLanguage = HttpContext.Items["AcceptLanguage"]?.ToString();
                            string scheme = HttpContext.Items["Scheme"]?.ToString();

                            // Gán các thông tin lấy được từ middleware vào ViewModel
                            model.browserInfo.UserAgent = userAgent;
                            model.browserInfo.AcceptLanguage = acceptLanguage;
                            model.browserInfo.Scheme = scheme;
                            string fingerprint = await _fingerprint.GenerateDeviceFingerprint(model.browserInfo);
                            if (fingerprint != null)
                            {
                                model.fingerprint.DeviceFingerprint = fingerprint;
                                bool checkFingerprint = await _fingerprintService.CheckFingerprint(model.fingerprint);
                                if (checkFingerprint)
                                {
                                    session.SetString("USERNAME", model.login.Username);
                                    session.SetString("AUTH", result.Token);

                                    var isExistShift = await _shift.GetAsync(result.Token);
                                    if (isExistShift != null && isExistShift.IsActive == true)
                                    {
                                        return RedirectToAction("Index", "Shift");
                                    }
                                    else
                                    {
                                        return RedirectToAction("Start", "Shift");
                                    }
                                }
                                else return RedirectToAction("Login", new { error = "Thiết bị đăng nhập không hợp lệ" });
                            }
                            else return RedirectToAction("Login");
                        }
                        return RedirectToAction("Index", "Shift", new { error = "Lỗi hệ thống: Không thể tải dữ liệu" });
                    }
                    else
                    {
                        return RedirectToAction("Login", new { error = "Tên đăng nhập hoặc mật khẩu không chính xác" });
                    }
                }
                else return RedirectToAction("Login", new { error = "Vui lòng điền tên đăng nhập và mật khẩu" });
            }
            else
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
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
				if (checkedToken != null && checkedToken.RoleName == "Employee")
				{
					var isCompleteShift = await _shift.GetAsync(token);
					if (isCompleteShift.IsActive == true)
						return RedirectToAction("Index","Shift", new { error = "Bạn cần phải kết thúc ca" });

                    var logout = await _auth.Logout(token);
					if (logout == 200)
					{
						session.Remove("AUTH");
                        token = session.GetString("AUTH");
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
