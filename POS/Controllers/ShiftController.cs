using Client_POS_Services.AuthServices;
using Client_Services.IncurredService;
using Client_Services.OrderServices;
using Client_Services.ShiftServices;
using Client_ViewModel.Incurred;
using Client_ViewModel.Order;
using Client_ViewModel.Shift;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Client_POS.Controllers
{


    public class ShiftController : Controller
    {
        private readonly IAuthService _auth;
        private readonly IShiftService _shift;
        private readonly IIncurredService _incurred;
        private readonly IOrderService _order;
        public ShiftController(IAuthService auth, IShiftService shift, IIncurredService incurred, IOrderService order)
        {
            _auth = auth;
            _shift = shift;
            _incurred = incurred;
            _order = order;
        }

        [HttpPost("CreateShift")]
        public async Task<IActionResult> CreateShift(CreateShiftViewModel shift)
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
                        var result = await _shift.CreateAsync(shift, token);
                        var parseToNumber = Convert.ToDouble(shift.BeginAmount.Replace("VNĐ", "")
                                   .Replace(".", "")
                                   .Replace(",", "")
                                   .Trim());
                        if(parseToNumber <= 10000)
                        {
                            return RedirectToAction(nameof(Start), new { error = result?.Message ?? "Lỗi hệ thống: Số tiền trong két tối thiểu 10.000 VNĐ" });
                        }
                        else
                        {
                            if (result?.IsSuccess == true)
                            {
                                return RedirectToAction(nameof(Index), new { success = "Tạo ca thành công" });
                            }
                            else
                            {
                                return RedirectToAction(nameof(Start), new { error = result?.Message ?? "Lỗi hệ thống: Tạo ca không thành công" });
                            }
                        }            
                    }
                    else
                    {
                        string message = string.Join(" & ", ModelState.Values
                         .SelectMany(v => v.Errors)
                         .Select(e => e.ErrorMessage));
                        return RedirectToAction(nameof(CreateShift), new { error = message });
                    }
                }
                else return RedirectToAction("Login", "Auth");
            }
            else return RedirectToAction("Login", "Auth");
        }

        [HttpPost("[controller]/EndShift")]
        public async Task<IActionResult> EndShift(string shiftId)
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
                        var result = await _shift.EndAsync(shiftId, token);
                        if (result?.IsSuccess == true)
                        {
                            var logout = await _auth.Logout(token);
                            if (logout == 200)
                            {
                                session.Remove("AUTH");
                                return RedirectToAction("Login", "Auth");
                            }
                            else return RedirectToAction(nameof(Index), new { error = result?.Message ?? "Lỗi hệ thống: Kết thúc không thành công" });
                        }
                        else
                        {
                            return RedirectToAction(nameof(Index), new { error = result?.Message ?? "Lỗi hệ thống: Kết thúc không thành công" });
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

        public async Task<IActionResult> Start(string error, string success)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkToken = await _auth.CheckTokenAsync(token);
                if (checkToken != null && checkToken.RoleName == "Employee")
                {
                    ShiftViewModel shift = await _shift.GetAsync(token);
                    if (shift == null)
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
                        return RedirectToAction("Index", "Shift");
                    }
                }
                else return RedirectToAction("Index", "Auth");
            }
            else return RedirectToAction("Index", "Auth");
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
                    ShiftViewModel shift = await _shift.GetAsync(token);
                    if (shift != null)
                    {
                        shift.CurrentAmount = shift.BeginAmount;
                        List<IncurredViewModel> inc = await _incurred.GetIncurreByIdAsync(shift.Id, token);
                        List<OrderViewModel> orderIncomplete = await _order.GetAllInCompleteOrderAsync(token);
                        List<OrderViewModel> ord = await _order.GetAllOrderByShiftIdIsComplete(shift.Id, token);

                        ord.AddRange(orderIncomplete);

                        /*        List<OrderForShiftViewModel> listOrdtmp = ord.Union(orderStatus).ToList();*/
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

                        DetailShiftViewModel result = new DetailShiftViewModel()
                        {
                            order = ord,
                            incurred = inc,
                            shift = shift
                        };
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
                    return RedirectToAction("Start", "Shift", new { error = "Vui Lòng Bắt Đầu Ca" });
                }
                else
                {
                    return RedirectToAction("Login", "Auth", new { error = "Vui Lòng Đăng Nhập" });
                }
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }

        }
    }
}
