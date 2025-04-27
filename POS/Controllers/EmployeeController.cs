using Client_Manager_MVC.Utilities;
using Client_POS_Services.AuthServices;
using Client_Services.EmployeeServices;
using Client_ViewModel.Employee;
using Client_ViewModel.Image;
using Client_ViewModel.Manager;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Client_POS.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employee;
        private readonly IAuthService _auth;
        public EmployeeController(IEmployeeService employee, IAuthService auth)
        {
            _auth = auth;
            _employee = employee;
        }

        public async Task<IActionResult> Profile(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    var profile = new Client_ViewModel.Employee.ProfileViewModel();
                    profile.Employee = await _employee.GetEmployeeAsync(checkedToken.AccountId, token);
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
                        return RedirectToAction("Index", "Home", new { error = "Lỗi hệ thống: Không thể tải dữ liệu" });
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
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    if (ModelState.IsValid)
                    {
                        var result = await _employee.UpdateEmployeeAsync(employee.UpdateEmployee, token);
                        if (result?.IsSuccess == true)
                        {
                            return RedirectToAction("Profile", "Employee", new { success = "Cập nhật hồ sơ thành công" });
                        }
                        else
                        {
                            return RedirectToAction("Profile", "Employee", new { error = result?.Message?? "Lỗi hệ thống: Cập nhật không thành công" });
                        }
                    }
                    else
                    {
                        employee.Employee = await _employee.GetEmployeeAsync(employee.UpdateEmployee.AccId, token);
                        ViewBag.Error = "Vui lòng kiểm tra lại các thông tin đã nhập";
                        return View("Profile", employee);
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
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    if (ModelState.IsValid)
                    {
                        var result = await _employee.ChangePasswordAsync(profile.ChangePassword, token);
                        if (result?.IsSuccess == true)
                        {
                            return RedirectToAction("Profile", "Employee", new { accId = profile.ChangePassword.AccId, success = "Cập nhật mật khẩu thành công" });
                        }
                        else
                        {
                            return RedirectToAction("Profile", "Employee", new { accId = profile.ChangePassword.AccId, error = result?.Message ?? "Lỗi hệ thống" });
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Vui lòng kiểm tra lại các thông tin đã nhập";
                        profile.Employee = await _employee.GetEmployeeAsync(profile.ChangePassword.AccId, token);
                        profile.UpdateEmployee = UpdateEmployeeViewModel.Create(profile.Employee);
                        return View(checkedToken.AccountId == profile.ChangePassword.AccId ? "Profile" : "Profile", profile);
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
                if (checkedToken != null && checkedToken.RoleName == "Employee")
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
                                return RedirectToAction("Profile", "Employee", routeValues);
                            }
                            bool isUploaded = await _employee.UploadImage(new CreateImageViewModel { Images = profile.ChangeImage.Images, ImgSetId = profile.ChangeImage.ImageSetId }, token);
                            if (isUploaded)
                            {
                                var routeValues = new { accId = profile.ChangeImage.AccId, success = "Cập nhật hình ảnh thành công" };
                                return RedirectToAction("Profile", "Employee", routeValues);
                            }
                            else
                            {
                                var routeValues = new { accId = profile.ChangeImage.AccId, error = "Cập nhật hình ảnh không thành công" };
                                return RedirectToAction("Profile", "Employee", routeValues);
                            }
                        }
                        else
                        {
                            var routeValues = new { accId = profile.ChangeImage.AccId, error = "Vui lòng chọn hình ảnh" };
                            return RedirectToAction("Profile", "Employee", routeValues);
                        }
                    }
                    else
                    {
                        var routeValues = new { accId = profile.ChangeImage.AccId, error = "Vui lòng kiểm tra lại hình ảnh đã chọn" };
                        return RedirectToAction("Profile", "Employee", routeValues);
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
                if (checkedToken != null && checkedToken.RoleName == "Employee")
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
