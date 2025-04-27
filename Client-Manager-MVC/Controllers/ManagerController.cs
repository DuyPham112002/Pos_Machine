using Client_Manager_MVC.Utilities;
using Client_Manager_Services;
using Client_Manager_Services.AuthServices;
using Client_Manager_Services.ManagerService;
using Client_ViewModel.Image;
using Client_ViewModel.Manager;
using Client_ViewModel.Token;
using Cosplane_API_ViewModel.Brand;
using Microsoft.AspNetCore.Mvc;

namespace Client_Manager_MVC.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IManagerService _manager;
        private readonly IAuthService _auth;
        private readonly IConfiguration _configuration;
        private readonly IBrandService _brand;


        public ManagerController(IManagerService manager, IAuthService auth, IConfiguration configuration, IBrandService brand)
        {
            _manager = manager;
            _auth = auth;
            _configuration = configuration;
            _brand = brand;
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
                    var results = await _manager.GetAllByAccIdAsync(checkedToken.AccountId, token);
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

        public async Task<IActionResult> Profile(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    var profile = new ProfileViewModel();
                    profile.Manager = await _manager.GetManagerAsync(checkedToken.AccountId, token);
                    if (profile.Manager != null)
                    {
                        profile.UpdateManager = UpdateManagerViewModel.Create(profile.Manager);
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
                    else
                    {
                        return RedirectToAction("Index", "Home", new { error = "Lỗi hệ thống: Không thể tải dữ liệu" });
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        [HttpGet("[Controller]/Detail/{accId}")]
        public async Task<IActionResult> Detail(string accId, string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    var profile = new ProfileViewModel();
                    profile.Manager = await _manager.GetManagerAsync(accId, token);
                    if (profile.Manager != null)
                    {
                        profile.UpdateManager = UpdateManagerViewModel.Create(profile.Manager);
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
                    else
                    {
                        return RedirectToAction("Index", "Home", new { error = "Lỗi hệ thống: Không thể tải dữ liệu" });
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        public async Task<IActionResult> Create(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
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

        [HttpPost("[controller]/Create")]
        public async Task<IActionResult> Create(CreateManagerViewModel model)
        {
            var token = HttpContext.Session.GetString("AUTH");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var checkedToken = await _auth.CheckTokenAsync(token);
            if (checkedToken?.RoleName != "Manager") return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Vui lòng kiểm tra lại các thông tin đã nhập";
                return View("Create", model);
            }

            if (model.Account.Password != model.Account.ConfirmPassword)
            {
                ViewBag.Error = "Mật khẩu không trùng khớp. Vui lòng kiểm tra lại";
                return View("Create", model);
            }
            string brandName = _configuration.GetSection("Brand").Value;
            GetBrandIdByBrandNameAPIViewModel brandId = await _brand.GetBrandIdAsync(brandName);
            if (brandId == null)
            {
                ViewBag.Error = "Đã có lỗi hệ thống xảy ra";
                return View("Create", model);
            }
            model.brandId = brandId.BrandId;
            var create = await _manager.CreateAsync(model, token);
            if (create?.IsSuccess == true)
            {
                return RedirectToAction(nameof(Create), new { success = "Tạo quản lý thành công" });
            }

            ViewBag.Error = create?.Message ?? "Đã có lỗi hệ thống xảy ra";
            return View("Create", model);
        }

        [HttpPost("[controller]/UpdateActive")]
        public async Task<IActionResult> UpdateActive(string accId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    var result = await _manager.UpdateActiveAsync(accId, token);
                    if (result)
                    {
                        return RedirectToAction("Index", new { success = "Cập nhật trạng thái thành công" });
                    }
                    else
                    {
                        return RedirectToAction("Index", new { error = "Cập nhật trạng thái không thành công" });
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        [HttpPost("[controller]/Update")]
        public async Task<IActionResult> Update(ProfileViewModel profile)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    if (ModelState.IsValid)
                    {
                        var result = await _manager.UpdateManagerAsync(profile.UpdateManager, token);
                        string action = (checkedToken.AccountId == profile.UpdateManager.AccId) ? "Profile" : "Detail";
                        if (result?.IsSuccess == true)
                        {
                            var routeValues = action.Equals("Profile", StringComparison.OrdinalIgnoreCase)
                                ? new { accId = string.Empty, success = "Cập nhật hồ sơ thành công" }
                                : new { accId = profile.UpdateManager.AccId, success = "Cập nhật hồ sơ thành công" };
                            return RedirectToAction(action, "Manager", routeValues);
                        }
                        else
                        {
                            var routeValues = action.Equals("Profile", StringComparison.OrdinalIgnoreCase)
                              ? new { accId = string.Empty, error = result?.Message ?? "Lỗi hệ thống: Cập nhật không thành công" }
                              : new { accId = profile.UpdateManager.AccId, error = result?.Message ?? "Lỗi hệ thống: Cập nhật không thành công" };
                            return RedirectToAction(action, "Manager", routeValues);
                        }
                    }
                    else
                    {
                        profile.Manager = await _manager.GetManagerAsync(profile.UpdateManager.AccId, token);
                        ViewBag.Error = "Vui lòng kiểm tra lại các thông tin đã nhập";
                        return View(checkedToken.AccountId == profile.UpdateManager.AccId ? "Profile" : "Detail", profile);
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        [HttpPost("[controller]/ChangePassword")]
        public async Task<IActionResult> ChangePassword(ProfileViewModel profile)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    if (ModelState.IsValid)
                    {
                        var result = await _manager.ChangePasswordAsync(profile.ChangePassword, token);
                        string action = (checkedToken.AccountId == profile.ChangePassword.AccId) ? "Profile" : "Detail";
                        if (result?.IsSuccess == true)
                        {
                            var routeValues = action.Equals("Profile", StringComparison.OrdinalIgnoreCase)
                                ? new { accId = string.Empty, success = "Cập nhật mật khẩu thành công" }
                                : new { accId = profile.ChangePassword.AccId, success = "Cập nhật mật khẩu thành công" };
                            return RedirectToAction(action, "Manager", routeValues);
                        }
                        else
                        {
                            var routeValues = action.Equals("Profile", StringComparison.OrdinalIgnoreCase)
                              ? new { accId = string.Empty, error = result?.Message ?? "Lỗi hệ thống" }
                              : new { accId = profile.ChangePassword.AccId, error = result?.Message ?? "Lỗi hệ thống" };
                            return RedirectToAction(action, "Manager", routeValues);
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Vui lòng kiểm tra lại các thông tin đã nhập";
                        profile.Manager = await _manager.GetManagerAsync(profile.ChangePassword.AccId, token);
                        profile.UpdateManager = UpdateManagerViewModel.Create(profile.Manager);
                        return View(checkedToken.AccountId == profile.ChangePassword.AccId ? "Profile" : "Detail", profile);
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        [HttpPost("[controller]/ChangeImage")]
        public async Task<IActionResult> ChangeImage(ProfileViewModel profile)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    string action = (checkedToken.AccountId == profile.ChangeImage.AccId) ? "Profile" : "Detail";
                    if (ModelState.IsValid && action != string.Empty)
                    {
                        string messageError = string.Empty;
                        if (profile.ChangeImage.Images != null && profile.ChangeImage.Images.Count() > 0)
                        {
                            string[] permittedExtensions = { ".png", ".jpg", ".jpeg" };
                            //check image
                            foreach (IFormFile image in profile.ChangeImage.Images)
                            {

                                using (var memoryStream = new MemoryStream())
                                {
                                    await image.CopyToAsync(memoryStream);
                                    if (memoryStream.Length == 0)
                                    {
                                        messageError = "File ảnh có thể bị lỗi. Vui lòng kiểm tra lại";
                                        break;
                                    }
                                    if (!FileHelper.IsValidFileExtensionAndSignature(image.FileName, memoryStream, permittedExtensions))
                                    {
                                        messageError = "Vui lòng upload .jpg, .png, .jpeg";
                                        break;
                                    }
                                }
                                if (image.Length > (2 * 1024 * 1024))
                                {
                                    messageError = "Vui lòng upload ảnh nhỏ hơn 2MB";
                                    break;
                                }
                            }
                            if (messageError != string.Empty)
                            {
                                var routeValues = action.Equals("Profile", StringComparison.OrdinalIgnoreCase)
                                      ? new { accId = string.Empty, error = messageError }
                                      : new { accId = profile.ChangeImage.AccId, error = messageError };
                                return RedirectToAction(action, "Manager", routeValues);
                            }
                            bool isUploaded = await _manager.UploadImage(new CreateImageViewModel { Images = profile.ChangeImage.Images, ImgSetId = profile.ChangeImage.ImageSetId }, token);
                            if (isUploaded)
                            {
                                var routeValues = action.Equals("Profile", StringComparison.OrdinalIgnoreCase)
                                     ? new { accId = string.Empty, success = "Cập nhật hình ảnh thành công" }
                                     : new { accId = profile.ChangeImage.AccId, success = "Cập nhật hình ảnh thành công" };
                                return RedirectToAction(action, "Manager", routeValues);
                            }
                            else
                            {
                                var routeValues = action.Equals("Profile", StringComparison.OrdinalIgnoreCase)
                                                                    ? new { accId = string.Empty, error = "Cập nhật hình ảnh không thành công" }
                                                                    : new { accId = profile.ChangeImage.AccId, error = "Cập nhật hình ảnh không thành công" };
                                return RedirectToAction(action, "Manager", routeValues);
                            }
                        }
                        else
                        {
                            var routeValues = action.Equals("Profile", StringComparison.OrdinalIgnoreCase)
                                    ? new { accId = string.Empty, error = "Vui lòng chọn hình ảnh" }
                                    : new { accId = profile.ChangeImage.AccId, error = "Vui lòng chọn hình ảnh" };
                            return RedirectToAction(action, "Manager", routeValues);
                        }
                    }
                    else
                    {
                        var routeValues = action.Equals("Profile", StringComparison.OrdinalIgnoreCase)
                                    ? new { accId = string.Empty, error = "Vui lòng kiểm tra lại hình ảnh đã chọn" }
                                    : new { accId = profile.ChangeImage.AccId, error = "Vui lòng kiểm tra lại hình ảnh đã chọn" };
                        return RedirectToAction(action, "Manager", routeValues);
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        [HttpGet("[controller]/document/{filename}")]
        public async Task<IActionResult> GetDocument(string filename)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Manager")
                {
                    byte[] documentData = await _manager.GetDocument(filename, token);
                    if (documentData != null)
                    {
                        return File(documentData, "image/jpeg");
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }
    }
}
