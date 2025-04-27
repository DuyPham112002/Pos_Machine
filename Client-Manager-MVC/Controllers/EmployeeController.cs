using Client_Employee_Services.Employee;
using Client_Manager_MVC.Utilities;
using Client_Manager_Services;
using Client_Manager_Services.AuthServices;
using Client_ViewModel.Employee;
using Client_ViewModel.Image;
using Client_ViewModel.Token;
using Cosplane_API_ViewModel.Brand;
using Microsoft.AspNetCore.Mvc;

namespace Client_employee_MVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IEmployeeService _employee;
        private readonly IAuthService _auth;
        private readonly IBrandService _brand;
        public EmployeeController(IConfiguration configuration, IEmployeeService employee, IAuthService auth, IBrandService brand)
        {
            _configuration = configuration;
            _auth = auth;
            _employee = employee;
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
                    var results = await _employee.GetAllByAccIdAsync(checkedToken.AccountId, token);
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
                    var profile = new Client_ViewModel.Employee.ProfileViewModel();
                    profile.Employee = await _employee.GetEmployeeAsync(accId, token);
                    if (profile.Employee != null)
                    {
                        profile.UpdateEmployee = UpdateEmployeeViewModel.Create(profile.Employee);
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
                        return RedirectToAction("Index", "Employee", new { error = "Lỗi hệ thống: Không thể tải dữ liệu" });
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
        public async Task<IActionResult> Create(Client_ViewModel.Employee.CreateEmployeeViewModel model)
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
            var create = await _employee.CreateAsync(model, token);
            if (create?.IsSuccess == true)
            {
                return RedirectToAction(nameof(Create), new { success = "Tạo nhân viên thành công" });
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
                    var result = await _employee.UpdateActiveAsync(accId, token);
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
        public async Task<IActionResult> Update(Client_ViewModel.Employee.ProfileViewModel employee)
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
                        var result = await _employee.UpdateEmployeeAsync(employee.UpdateEmployee, token);
                        if (result?.IsSuccess == true)
                        {
                            return RedirectToAction("Detail", "Employee", new { accId = employee.UpdateEmployee.AccId, success = "Cập nhật hồ sơ thành công" });
                        }
                        else
                        {
                            return RedirectToAction("Detail", "Employee", new { accId = employee.UpdateEmployee.AccId, error = result?.Message ?? "Lỗi hệ thống: Cập nhật không thành công" });
                        }
                    }
                    else
                    {
                        employee.Employee = await _employee.GetEmployeeAsync(employee.UpdateEmployee.AccId, token);
                        ViewBag.Error = "Vui lòng kiểm tra lại các thông tin đã nhập";
                        return View("Detail", employee);
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        [HttpPost("[controller]/ChangePassword")]
        public async Task<IActionResult> ChangePassword(Client_ViewModel.Employee.ProfileViewModel profile)
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
                        var result = await _employee.ChangePasswordAsync(profile.ChangePassword, token);
                        if (result?.IsSuccess == true)
                        {
                            return RedirectToAction("Detail", "Employee", new { accId = profile.ChangePassword.AccId, success = "Thay đổi mật khẩu thành công" });
                        }
                        else
                        {
                            return RedirectToAction("Detail", "Employee", new { accId = profile.ChangePassword.AccId, error = result?.Message ?? "Lỗi hệ thống" });
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Vui lòng kiểm tra lại các thông tin đã nhập";
                        profile.Employee = await _employee.GetEmployeeAsync(profile.ChangePassword.AccId, token);
                        profile.UpdateEmployee = UpdateEmployeeViewModel.Create(profile.Employee);
                        return View(checkedToken.AccountId == profile.ChangePassword.AccId ? "Profile" : "Detail", profile);
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        [HttpPost("[controller]/ChangeImage")]
        public async Task<IActionResult> ChangeImage(Client_ViewModel.Employee.ProfileViewModel profile)
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
                                var routeValues = new { accId = profile.ChangeImage.AccId, error = messageError };
                                return RedirectToAction("Detail", "Employee", routeValues);
                            }
                            bool isUploaded = await _employee.UploadImage(new CreateImageViewModel { Images = profile.ChangeImage.Images, ImgSetId = profile.ChangeImage.ImageSetId }, token);
                            if (isUploaded)
                            {
                                var routeValues = new { accId = profile.ChangeImage.AccId, success = "Cập nhật hình ảnh thành công" };
                                return RedirectToAction("Detail", "Employee", routeValues);
                            }
                            else
                            {
                                var routeValues = new { accId = profile.ChangeImage.AccId, error = "Cập nhật hình ảnh không thành công" };
                                return RedirectToAction("Detail", "Employee", routeValues);
                            }
                        }
                        else
                        {
                            var routeValues = new { accId = profile.ChangeImage.AccId, error = "Vui lòng chọn hình ảnh" };
                            return RedirectToAction("Detail", "Employee", routeValues);
                        }
                    }
                    else
                    {
                        var routeValues = new { accId = profile.ChangeImage.AccId, error = "Vui lòng kiểm tra lại hình ảnh đã chọn" };
                        return RedirectToAction("Detail", "Employee", routeValues);
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
                    byte[] documentData = await _employee.GetDocument(filename, token);
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
