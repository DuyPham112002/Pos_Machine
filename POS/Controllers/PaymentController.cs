using Client_POS_Services.AuthServices;
using Client_Services.OrderDetailServices;
using Client_Services.OrderServices;
using Client_Services.PaymentService;
using Client_Services.ProductServices;
using Client_ViewModel.Order;
using Client_ViewModel.Payment;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Mvc;

namespace Client_POS.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IAuthService _auth;
        private readonly IOrderService _odrder;
        private readonly IOrderDetailService _detail;
        private readonly IProductService _product;
        private readonly IPaymentService _payment;
        public PaymentController(IAuthService auth, IOrderService order, IOrderDetailService detail, IProductService product, IPaymentService payment)
        {
            _auth = auth;
            _odrder = order;
            _detail = detail;
            _product = product;
            _payment = payment;
        }

        [HttpGet("[controller]/Checkout/{orderId}")]
        public async Task<IActionResult> Checkout(string orderId, string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    CreatePaymentViewModel model = new CreatePaymentViewModel();
                    model.Order = await _odrder.GetAsync(orderId, token);
                   if(model.Order.Status == 1)
                    {
                        model.Orders = await _detail.GetAllAsync(orderId, token);
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
                        return RedirectToAction("Detail", "Order", new { orderId = model.Order.Id });
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



        [HttpPost("[controller]/Create")]
        public async Task<IActionResult> Create(CreatePaymentViewModel model)
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
                        var result = await _payment.CreateAsync(model.Create, token);
                        if (result?.IsSuccess == true)
                        {
                            return RedirectToAction("Invoice", "Order", new { orderId = model.Create.OrderId, success = "Thanh Toán Thành Công" });
                        }
                        else
                        {
                            return RedirectToAction("Checkout", "Payment", new { orderId = model.Create.OrderId, error = result?.Message ?? "Lỗi hệ thống: Thanh Toán không thành công" });
                        }
                    }
                    else
                    {            
                        ViewBag.Error = "Vui lòng kiểm tra lại các thông tin đã nhập";
                        model.Order = await _odrder.GetAsync(model.Create.OrderId, token);
                        model.Orders = await _detail.GetAllAsync(model.Create.OrderId, token);
                        return View("Checkout", model);
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
    }
}
