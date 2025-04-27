using Client_ViewModel.Fingerprint;
using Cosplane_API_ViewModel.Brand;
using Cosplane_API_ViewModel.Device;
using Cosplane_API_ViewModel.Token;
using Cosplane_MVC_Service.AuthService;
using Cosplane_MVC_Service.BrandService;
using Cosplane_MVC_Service.DeviceService;
using Cosplane_MVC_Service.FingerprintService;
using Cosplane_MVC_ViewModel.Device;
using Microsoft.AspNetCore.Mvc;

namespace Cosplane_MVC.Controllers
{
    public class DeviceController : Controller
    {
        //fields
        private readonly IAuthService _auth;
        private readonly IDeviceService _device;
        private readonly IBrandService _brand;
        private readonly IFingerprintService _fingerprint;

        //constructor with params
        public DeviceController(IAuthService auth, IDeviceService device, IBrandService brand, IFingerprintService fingerprint)
        {
            _auth = auth;
            _device = device;
            _brand = brand;
            _fingerprint = fingerprint;
        }

        //methods
        //Funtion to process create service
        [HttpPost("[controller]/Create")]
        public async Task<IActionResult> Create(GetBrandForDeviceMVCViewModel model)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    if (ModelState.IsValid)
                    {
                        bool created = await _device.CreateAsync(model.CreateDeviceModel, token);
                        if (created)
                        {
                            return RedirectToAction(nameof(Create), new
                            {
                                success = "Tạo thiết bị mới thành công"
                            });
                        }
                        else
                        {
                            return RedirectToAction(nameof(Create), new
                            {
                                error = "Không thể tạo thiết bị mới"
                            });
                        }
                    }
                    else
                    {
                        string message = string.Join(" & ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                        return RedirectToAction(nameof(Create), new { error = message });
                    }
                }
                return RedirectToAction("Login", "Auth");
            }
            return RedirectToAction("Login", "Auth");
        }

        //function to show the function create to view
        [HttpGet("[controller]/Create")]
        public async Task<IActionResult> Create(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null)
                {
                    List<GetAllBrandAPIViewModel> result = await _brand.GetAllAsync(token);
                    if (result != null && result.Count > 0)
                    {
                        GetBrandForDeviceMVCViewModel viewModel = new GetBrandForDeviceMVCViewModel
                        {
                            GetAllBrandModel = result,
                            CreateDeviceModel = new CreateDeviceAPIViewModel()
                        };
                        if (error != null)
                        {
                            ViewBag.Error = error;
                        }
                        if (success != null)
                        {
                            ViewBag.Success = success;
                        }
                        return View(viewModel);
                    }
                    else return RedirectToAction("Create", "Device");
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

        // function to show get fingerprint
        [HttpPost("[controller]/Fingerprint/Create")]
        public async Task<IActionResult> Fingerprint([FromBody] GetBrowserInfoViewModel model)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedtoken = await _auth.CheckTokenAsync(token);
                if (checkedtoken != null)
                {
                    // Lấy thông tin từ middleware
                    string userAgent = HttpContext.Items["UserAgent"]?.ToString();
                    string acceptLanguage = HttpContext.Items["AcceptLanguage"]?.ToString();
                    string scheme = HttpContext.Items["Scheme"]?.ToString();

                    // Gán các thông tin lấy được từ middleware vào ViewModel
                    model.UserAgent = userAgent;
                    model.AcceptLanguage = acceptLanguage;
                    model.Scheme = scheme;
                    string fingerprint = await _fingerprint.GenerateDeviceFingerprint(model);
                    return Ok(fingerprint);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }
        // function to show get fingerprint
        [HttpGet("[controller]/Fingerprint")]
        public async Task<IActionResult> Fingerprint(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedtoken = await _auth.CheckTokenAsync(token);
                if (checkedtoken != null)
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

        // function to show all brands
        [HttpGet("[controller]/Brand")]
        public async Task<IActionResult> Index(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    List<GetAllBrandAPIViewModel> result = await _device.GetAllAsync(token);
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
                        return RedirectToAction("Index", "Home", new { error = "Không tìm thấy bất kỳ thương hiệu nào" });
                    }
                }
                else return RedirectToAction("Index", "Auth");
            }
            else return RedirectToAction("Index", "Auth");
        }

        // function to show all device by brandId
        [HttpGet("[controller]/{brandId}")]
        public async Task<IActionResult> ListDevices(string brandId, string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    List<GetAllDeviceByBrandAPIViewModel> result = await _device.GetListDeviceAsync(token, brandId);
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
                        return RedirectToAction("Index", "Device", new { error = "Không tìm thấy bất kỳ thương hiệu nào" });
                    }
                }
                else return RedirectToAction("Index", "Auth");
            }
            else return RedirectToAction("Index", "Auth");
        }
        //function to process update a device
        [HttpPost("[controller]/UpdateDevice")]
        public async Task<IActionResult> UpdateDevice(GetBrandUpdateDeviceMVCViewModel model)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    bool result = await _device.UpdateAsync(model.UpdateDevice, token);
                    if (result)
                    {
                        return RedirectToAction("Update", "Device", new { deviceId = model.UpdateDevice.DeviceId, success = "Cập nhật thông tin thiết bị thành công" });

                    }
                    return RedirectToAction("Update", "Device", new { deviceId = model.UpdateDevice.DeviceId, error = "Cập nhật thông tin thiết bị gặp lỗi" });

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

        //function to show the update function
        [HttpGet("[controller]/Update/{deviceId}")]
        public async Task<IActionResult> Update(string deviceId, string success, string error)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null)
                {
                    GetDeviceByIdAPIViewModel device = await _device.GetDeviceAsync(token, deviceId);
                    if (device != null)
                    {
                        List<GetAllBrandAPIViewModel> result = await _brand.GetAllAsync(token);
                        if (result != null && result.Count > 0)
                        {
                            GetBrandUpdateDeviceMVCViewModel viewModel = new GetBrandUpdateDeviceMVCViewModel
                            {
                                GetAllBrandToUpdateModel = result,
                                UpdateDevice = new UpdateDeviceAPIViewModel()
                                {
                                    DeviceId = deviceId,
                                    BrandId = device.BrandId,
                                    DeviceFingerPrint = device.DeviceFingerprint
                                }
                            };
                            if (error != null)
                            {
                                ViewBag.Error = error;
                            }
                            if (success != null)
                            {
                                ViewBag.Success = success;
                            }
                            return View(viewModel);
                        }
                        else return RedirectToAction("Index", "Device");
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index), new { error = "Không tìm thấy thiết bị" });
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

        //function to process the function of delete a brand
        [HttpGet("[controller]/Delete/{deviceId}")]
        public async Task<IActionResult> Delete(string deviceId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    GetDeviceByIdAPIViewModel checkDevice = await _device.GetDeviceAsync(token, deviceId);
                    if (checkDevice != null)
                    {
                        bool result = await _device.DeleteAsync(token, deviceId);
                        if (result)
                        {
                            return RedirectToAction($"{checkDevice.BrandId}", "Device", new { success = "Xóa thiết bị thành công" });
                        }
                        return RedirectToAction($"{checkDevice.BrandId}", "Device", new { error = "Xóa thiết bị gặp lỗi" });
                    }
                    else return RedirectToAction("Index", "Device", new { error = "Không tìm thấy thiết bị" });
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

        //function to process the function of activate a brand
        [HttpGet("[controller]/Activate/{deviceId}")]
        public async Task<IActionResult> Activate(string deviceId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    GetDeviceByIdAPIViewModel checkDevice = await _device.GetDeviceAsync(token, deviceId);
                    if (checkDevice != null)
                    {
                        bool result = await _device.ActivateAsync(token, deviceId);
                        if (result)
                        {
                            return RedirectToAction($"{checkDevice.BrandId}", "Device", new { success = "Kích hoạt lại thiết bị thành công" });
                        }
                        return RedirectToAction($"{checkDevice.BrandId}", "Device", new { error = "Kích hoạt thiết bị gặp lỗi" });
                    }
                    else return RedirectToAction("Index", "Device", new { error = "Không tìm thấy thiết bị" });
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

        //function to get a device by deviceId
        [HttpGet("[controller]/Detail/{deviceId}")]
        public async Task<IActionResult> Detail(string deviceId, string success, string error)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "admin")
                {
                    GetDeviceByIdAPIViewModel checkDevice = await _device.GetDeviceAsync(token, deviceId);
                    if (checkDevice != null)
                    {
                        if (error != null)
                        {
                            ViewBag.Error = error;
                        }
                        if (success != null)
                        {
                            ViewBag.Success = success;
                        }
                        return View(checkDevice);
                    }
                    else return RedirectToAction("Index", "Device", new { error = "Không tìm thấy thiết bị" });
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
