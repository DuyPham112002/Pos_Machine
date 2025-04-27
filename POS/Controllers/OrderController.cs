using Client_POS_Services;
using Client_POS_Services.AuthServices;
using Client_Services.CategoryServices;
using Client_Services.OrderDetailServices;
using Client_Services.OrderServices;
using Client_Services.PaymentService;
using Client_Services.ProductServices;
using Client_Services.SettingService;
using Client_Services.SubCategoryServices;
using Client_ViewModel.Brand;
using Client_ViewModel.Category;
using Client_ViewModel.Order;
using Client_ViewModel.Product;
using Client_ViewModel.Setting;
using Client_ViewModel.SubCategory;
using Client_ViewModel.Token;
using Cosplane_API_ViewModel.Brand;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Client_POS.Controllers
{
    public class OrderController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _auth;
        private readonly ICategoryService _category;
        private readonly ISubCategoryService _subcategory;
        private readonly IProductService _product;
        private readonly IOrderService _order;
        private readonly IOrderDetailService _detail;
        private readonly IPaymentService _payment;
        private readonly ISettingService _setting;
        private readonly IBrandService _brand;
        public OrderController(IConfiguration config, IAuthService auth, ICategoryService category, ISubCategoryService subcategory, IProductService product, IOrderService order, IPaymentService payment, IOrderDetailService detail, ISettingService setting, IBrandService brand)
        {
            _config = config;
            _auth = auth;
            _category = category;
            _subcategory = subcategory;
            _product = product;
            _order = order;
            _payment = payment;
            _detail = detail;
            _setting = setting;
            _brand = brand;
        }

        public async Task<IActionResult> Index(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    var model = await _order.GetAllCompleteOrderAsync(token);

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

        [HttpGet("[Controller]/CompleteOrderQuantity")]
        public async Task<IActionResult> GetQuantityCompleteOrder(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    var model = await _order.GetAllCompleteOrderAsync(token);
                    

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
                        return Ok(model.Count);
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

        public async Task<IActionResult> InCompleteOrder(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    var order = await _order.GetAllInCompleteOrderAsync(token);

                    if (order != null)
                    {
                        if (error != null)
                        {
                            ViewBag.Error = error;
                        }
                        if (success != null)
                        {
                            ViewBag.Success = success;
                        }
                        return View(order);
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

        [HttpGet("[Controller]/InCompleteOrderQuantity")]
        public async Task<IActionResult> GetQuantityInCompleteOrder(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    var model = await _order.GetAllInCompleteOrderAsync(token);
                    int numberofOrder = model.Count;

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
                        return Ok(numberofOrder);
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

        public async Task<IActionResult> CanceledOrder(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    var order = await _order.GetAllCanceledOrderAsync(token);

                    if (order != null)
                    {
                        if (error != null)
                        {
                            ViewBag.Error = error;
                        }
                        if (success != null)
                        {
                            ViewBag.Success = success;
                        }
                        return View(order);
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

        [HttpGet("[Controller]/CanceledOrderQuantity")]
        public async Task<IActionResult> GetQuantityCanceledOrder(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    var model = await _order.GetAllCanceledOrderAsync(token);


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
                        return Ok(model.Count);
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

        [HttpGet("[controller]/Detail/{orderId}")]
        public async Task<IActionResult> Detail(string orderId, string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    DetailOrderViewModel model = await _order.GetDetailAsync(orderId, token);
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
                        return RedirectToAction("Index", "Order", new { error = "Lỗi hệ thống: Không thể tải dữ liệu" });
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

        [HttpGet("[controller]/Invoice/{orderId}")]
        public async Task<IActionResult> Invoice(string orderId, string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    InvoiceOrderViewModel model = new InvoiceOrderViewModel();
                    model.Setting = await GetSettingAsync();
                    if (model.Setting != null)
                    {
                        model.Order = await _order.GetAsync(orderId, token);
                        if (model.Order != null)
                        {
                            model.Orders = await _detail.GetAllAsync(orderId, token);
                            if (model.Orders != null)
                            {
                                model.Payment = await _payment.GetAsync(orderId, token);
                                if (model.Payment != null)
                                {
                                    if (error != null)
                                    {
                                        ViewBag.Error = error;
                                    }
                                    if (success != null)
                                    {
                                        ViewBag.Success = success;
                                    }
                                    if (!string.IsNullOrEmpty(model.Setting.Device) && model.Setting.Device == "Window")
                                        return View("InvoiceWindow", model);
                                    else
                                        return View(model);
                                }
                            }
                        }
                        return RedirectToAction("Index", "Order", new { error = "Lỗi hệ thống: Không thể tải dữ liệu" });
                    }
                    else return RedirectToAction("Detail", "Order", new { orderId = orderId, error = "Không tìm thấy thông tin quán để in HĐ - liên hệ quản lý để kiểm tra" });
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

        [HttpPost("UpdateUnActived")]
        public async Task<IActionResult> UpdateUnActived(DetailOrderViewModel model)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    if (!ModelState.IsValid)
                    {
                        ViewBag.Error = "Vui lòng kiểm tra lại các thông tin đã nhập";
                        var detail = await _order.GetDetailAsync(model.Cancel.Id, token);
                        detail.Cancel = model.Cancel;
                        return View("Detail", detail);
                    }

                    var result = await _order.UpdateUnActiveAsync(model.Cancel, token);
                    if (result?.IsSuccess == true)
                    {
                        return RedirectToAction("Detail", "Order", new { orderId = model.Cancel.Id, success = "Hủy hóa đơn thành công" });
                    }
                    else
                    {
                        return RedirectToAction("Detail", "Order", new { orderId = model.Cancel.Id, error = result?.Message ?? "Đã có lỗi hệ thống xảy ra" });
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
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    CreateOrderViewModel model = new CreateOrderViewModel();
                    model.Categories = await _category.GetAllCategoryAsync(token);
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
                    return RedirectToAction("Login", "Auth");
                }
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderViewModel model)
        {
            var token = HttpContext.Session.GetString("AUTH");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var checkedToken = await _auth.CheckTokenAsync(token);
            if (checkedToken?.RoleName != "Employee") return RedirectToAction("Login", "Auth");

            if (model.OrderDetails == null || model.OrderDetails.Count() == 0)
            {
                return Json(new { error = true, message = "Vui lòng chọn sản phẩm để tạo hóa đơn" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { error = true, message = "Vui lòng kiểm tra lại thông tin hóa đơn" });
            }

            model.Order.Id = Guid.NewGuid().ToString();
            var create = await _order.CreateAsync(model, token);
            if (create?.IsSuccess == true)
            {
                return Json(new { success = true, message = "Tạo hóa đơn thành công", orderId = model.Order.Id });
            }

            return Json(new { error = true, message = create?.Message ?? "Đã có lỗi hệ thống xảy ra." });
        }


        [HttpGet("[controller]/Edit/{orderId}")]
        public async Task<IActionResult> Edit(string orderId, string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    UpdateOrderViewModel model = await _order.GetUpdateAsync(orderId, token);
                    if (model != null)
                    {
                        model.Categories = await _category.GetAllCategoryAsync(token);
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
                        return RedirectToAction("Index", "Order", new { error = "Lỗi hệ thống: Không thể tải dữ liệu" });
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

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateOrderViewModel model)
        {
            var token = HttpContext.Session.GetString("AUTH");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var checkedToken = await _auth.CheckTokenAsync(token);
            if (checkedToken?.RoleName != "Employee") return RedirectToAction("Login", "Auth");

            if (model.OrderDetails == null || model.OrderDetails.Count() == 0)
            {
                return Json(new { error = true, message = "Vui lòng chọn sản phẩm để cập nhật hóa đơn" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { error = true, message = "Vui lòng kiểm tra lại thông tin hóa đơn" });
            }

            var create = await _order.UpdateAsync(model, token);
            if (create?.IsSuccess == true)
            {
                return Json(new { success = true, message = "Cập nhật hóa đơn thành công" });
            }

            return Json(new { error = true, message = create?.Message ?? "Đã có lỗi hệ thống xảy ra." });
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
                    byte[] documentData = await _product.GetDocument(filename, token);
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

        private async Task<SettingViewModel> GetSettingAsync()
        {
            string brandName = _config.GetSection("Brand").Value;
            GetBrandIdByBrandNameAPIViewModel brand = await _brand.GetBrandIdAsync(brandName);
            if (brand != null)
            {
                ISession session = HttpContext.Session;
                string token = session.GetString("AUTH");
                SettingViewModel setting = await _setting.GetAsync(brand.BrandId, token);
                if (setting != null)
                {
                    setting.BrandName = brandName;
                    if (float.TryParse(_config["Print:Size"], out float result))
                    {
                        setting.Size = result;
                    }
                    setting.Device = _config.GetSection("Device").Value;
                }
                return setting;
            }
            return null;
        }
    }
}
