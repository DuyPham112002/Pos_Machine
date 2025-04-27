using Client_POS_Services.AuthServices;
using Client_Services.IncurredService;
using Client_Services.OrderServices;
using Client_Services.ShiftServices;
using Client_ViewModel.Incurred;
using Client_ViewModel.Order;
using Client_ViewModel.Shift;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Mvc;

namespace Client_POS.Controllers
{
    public class IncurredController : Controller
    {
        private readonly IAuthService _auth;
        private readonly IIncurredService _incurred;
        private readonly IShiftService _shift;
        private readonly IOrderService _order;
        public IncurredController(IAuthService auth, IIncurredService incurred, IShiftService shift, IOrderService order)
        {
            _auth = auth;
            _incurred = incurred;
            _shift = shift;
            _order = order;
        }

        [HttpPost("CreateIncurred")]
        public async Task<IActionResult> CreateIncurred(CreateIncurredViewModel inccure)
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
                        var shift = await _shift.GetAsync(token);
                        shift.CurrentAmount = shift.BeginAmount;
                        List<IncurredViewModel> inc = await _incurred.GetIncurreByIdAsync(shift.Id, token);
                        List<OrderViewModel> orderIncomplete = await _order.GetAllInCompleteOrderAsync(token);
                        List<OrderViewModel> ord = await _order.GetAllOrderByShiftIdIsComplete(shift.Id, token);

                        ord.AddRange(orderIncomplete);

                        if (inc != null)
                        {
                            shift.CurrentAmount += inc.Where(p => p.IsActive).Sum(p => p.Amount);

                        }
                        if (ord != null)
                        {
                            //Nếu order chưa thanh toán thì + tiền vào provisional
                            shift.provisional += ord.Where(o => o.Status == 1 || o.Status == 2).Sum(o => o.Total);
                            double tmpProvisional = shift.provisional + shift.CurrentAmount;
                            shift.provisional = tmpProvisional;
                            //Nếu order đã thanh toán thì + tiền vào Current
                            shift.CurrentAmount += ord.Where(o => o.Status == 2 && o.ShiftId == shift.Id).Sum(o => o.Total);

                        }
                        if (shift != null)
                        {
                            double tempAmount = Convert.ToDouble(inccure.Amount.Replace("VNĐ", "")
                                   .Replace(".", "")
                                   .Replace(",", "")
                                   .Trim());
                            double isNegativeAmount = shift.CurrentAmount + tempAmount;
                            if (isNegativeAmount < 0)
                            {
                                return RedirectToAction(nameof(Index), new { error = "Phát sinh không được lớn hơn số tiền hiện tại" });
                            }
                            else
                            {
                                var result = await _incurred.CreateIncurreAsync(inccure, token);

                                if (result?.IsSuccess == true)
                                {
                                    return RedirectToAction("Index", "Shift", new { success = "Tạo phát sinh thành công" });
                                }
                                else
                                {
                                    return RedirectToAction(nameof(Index), new { error = result?.Message ?? "Lỗi hệ thống: Tạo  phát sinh không thành công" });
                                }
                               
                            }
                        }
                        else
                        {
                                 return RedirectToAction("Login", "Auth",new {error = "Lỗi xảy ra khi lấy ca"});
                        }
                      
                    }
                    else
                    {
                        string message = string.Join(" & ", ModelState.Values
                         .SelectMany(v => v.Errors)
                         .Select(e => e.ErrorMessage));
                        return RedirectToAction(nameof(Index), new { error = message });
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        public async Task<IActionResult> Index(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkToken = await _auth.CheckTokenAsync(token);
                if (checkToken != null && checkToken.RoleName == "Employee")
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
                else return RedirectToAction("Index", "Auth");
            }
            else return RedirectToAction("Index", "Auth");
        }
    }
}
