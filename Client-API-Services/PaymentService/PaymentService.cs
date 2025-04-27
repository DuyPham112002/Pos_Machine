using Client_DBAccess.Dapper;
using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Order;
using Client_ViewModel.Payment;
using Client_ViewModel.Response;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Services.PaymentService
{
    public interface IPaymentService
    {
        Task<ResponseBase> CreateAsync(CreatePaymentAPIViewModel model, string accId);
        Task<ResponseValue<PaymentViewModel>> GetAsync(string orderId);
    }
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDapperContext _dapper;
        public PaymentService(IUnitOfWork uow, IDapperContext dapper)
        {
            _uow = uow;
            _dapper = dapper;
        }

        public async Task<ResponseBase> CreateAsync(CreatePaymentAPIViewModel model, string accId)
        {
            try
            {
                Order order = await _uow.Order.GetFirstOrDefaultAsync(p => p.Id == model.OrderId && p.Status == 1);
                Shift shift = await _uow.Shift.GetFirstOrDefaultAsync(a => a.Id == order.ShiftId && a.IsActive == false);
                if (order != null)
                {
                    if(shift != null)
                    {
                        shift.EndAmount = shift.EndAmount + order.Total;
                    }
                    if(double.TryParse(model.Received.Replace("VNĐ", "").Replace(".", "").Replace(",", "").Trim(), out double received))
                    {
                        if(received < model.Amount)
                            return ResponseBase.Result(false, 400, $"Số tiền thanh toán của khách hàng cần tối thiểu bằng số tiền ghi trên hóa đơn");
                        Payment payment = new Payment
                        {
                            Id = Guid.NewGuid().ToString(),
                            OrderId = model.OrderId,
                            PaymentMethod = model.PaymentMethod,
                            Amount = model.Amount,
                            Received = received,
                            Changed = received - model.Amount,
                            CreatedOn = DateTime.Now,
                            CreatedBy = order.CreatedBy
                        };
                        //Update order status
                        order.Status = 2;
                        order.ModifiedBy = accId;
                        order.ModifiedOn = DateTime.Now;
                        _uow.Order.Update(order);
                        await _uow.Payment.AddAsync(payment);
                        await _uow.SaveAsync();
                        //Update log activity
                        await ExcuteLogAcitivity(model.OrderId, "Finish", accId);
                        return ResponseBase.Result(true, 200);
                    }
                    return ResponseBase.Result(false, 404, "Lỗi hệ thống: Không thể thanh toán hóa đơn");
                }
                return ResponseBase.Result(false, 404, "Lỗi hệ thống: Không tìm thấy dữ liệu");
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ResponseValue<PaymentViewModel>> GetAsync(string orderId)
        {
            try
            {
                string query = $"select p.Id, P.PaymentMethod, P.Amount, P.Received, P.Changed, P.CreatedOn  from Payment as P with(nolock) where P.OrderId = '{orderId}'";
                using (var conection = _dapper.CreateConnection())
                {
                    PaymentViewModel order = await conection.QueryFirstAsync<PaymentViewModel>(query);
                    if (order != null)
                    {
                        return ResponseValue<PaymentViewModel>.Result(true, 200, order);
                    }
                    return ResponseValue<PaymentViewModel>.Result(false, 404, null);
                }
            }
            catch (Exception ex)
            {
                return ResponseValue<PaymentViewModel>.Result(false, 500, null);
            }
        }

        private async Task<bool> ExcuteLogAcitivity(string orderId, string logActivated, string accId)
        {
            string query = $"insert into OrderActivityLog (OrderId, LogActivated, CreatedOn, CreatedBy) values('{orderId}',N'{logActivated}',GETDATE(),'{accId}')";
            using (var conection = _dapper.CreateConnection())
            {
                try
                {
                    var result = await conection.ExecuteAsync(query);
                    return Convert.ToBoolean(result);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }
}
