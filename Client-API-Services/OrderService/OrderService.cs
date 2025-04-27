using Client_DBAccess.Dapper;
using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Order;
using Client_ViewModel.OrderDetail;
using Client_ViewModel.Product;
using Client_ViewModel.Response;
using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Client_API_Services.OrderService
{
    public interface IOrderService
    {
        Task<ResponseBase> CreateAsync(CreateOrderAPIViewModel create, string accId);
        Task<ResponseBase> UpdateAsync(UpdateOrderViewModel create, string accId);
        Task<ResponseValue<IEnumerable<OrderViewModel>>> GetAllAsync();
        Task<ResponseValue<OrderViewModel>> GetAsync(string orderId, string accId);
        Task<ResponseValue<UpdateOrderViewModel>> GetUpdateAsync(string orderId);
        Task<ResponseBase> UpdateActiveAsync(CancelOrderViewModel model, string accId);
		Task<ResponseValue<IEnumerable<OrderViewModel>>> GetAllInCompleteOrderAsync();
        Task<ResponseValue<IEnumerable<OrderViewModel>>> GetAllCompleteOrderAsync(string accId);
        Task<ResponseValue<IEnumerable<OrderViewModel>>> GetAllOrderByShiftIdIsComplete(string shiftId);
        Task<ResponseValue<IEnumerable<OrderViewModel>>> GetAllOrderCanceled(string accId);
       

    }
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDapperContext _dapper;
        public OrderService(IUnitOfWork uow, IDapperContext dapper)
        {
            _uow = uow;
            _dapper = dapper;
        }

        public async Task<ResponseBase> CreateAsync(CreateOrderAPIViewModel create, string accId)
        {
            try
            {
                var shift = await _uow.Shift.GetFirstOrDefaultAsync(p => p.AccId == accId && p.IsActive == true && p.TimeStart.Date == DateTime.Now.Date && p.TimeEnd == null);
                if(shift != null)
                {
                    string newCode = await GenerateCode();
                    Order newOrder = new Order
                    {
                        Id = create.Id,
                        Code = newCode,
                        ShiftId = shift.Id,
                        Total = create.Total,
                        Status = 1,
                        Note = create.Note,
                        CreatedOn = DateTime.Now,
                        CreatedBy = accId
                    };
                    await _uow.Order.AddAsync(newOrder);
                    await _uow.SaveAsync();
                    await ExcuteLogAcitivity(newOrder.Id, "Insert", accId);
                    return ResponseBase.Result(true, 200);
                }
                return ResponseBase.Result(false, 404, $"Lỗi hệ thống: Không tìm thấy dữ liệu ca để tạo hóa đơn");
            }
            catch (Exception ex)
            { 
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ResponseBase> UpdateAsync(UpdateOrderViewModel create, string accId)
        {
            try
            {
                Order current = await _uow.Order.GetFirstOrDefaultAsync(p => p.Id == create.Update.Id && p.Status == 1, "OrderDetails");
                if (current != null)
                {
                    
                    if (create.OrderDetails != null && create.OrderDetails.Any())
                    {
                        if (current.Total != create.Update.Total || current.Note != create.Update.Note)
                        {
                            current.Total = create.Update.Total;
                            current.Note = create.Update.Note;
                            current.ModifiedOn = DateTime.Now;
                            current.ModifiedBy = accId;
                            _uow.Order.Update(current);
                        }

                        List<OrderDetail> results = current.OrderDetails
                       .Where(p1 => p1.IsActive && !create.OrderDetails.Any(p2 => p2.Id == p1.Id))
                       .ToList();

                        //Delete
                        if (results != null && results.Any())
                        {
                            foreach (var item in results)
                            {
                                item.IsActive = false;
                                _uow.OrderDetail.Update(item);
                            }
                        }
                        foreach (var item in create.OrderDetails)
                        {
                            //Insert
                            if (string.IsNullOrEmpty(item.Id))
                            {
                                OrderDetail newOrder = new OrderDetail
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    AttributeId = !string.IsNullOrEmpty(item.AttributeId) ? item.AttributeId : null,
                                    OrderId = create.Update.Id,
                                    ProductId = item.ProductId,
                                    Quantity = item.Quantity,
                                    Price = item.Price,
                                    Subtotal = item.SubTotal,
                                    CreatedOn = DateTime.Now,
                                    CreatedBy = accId,
                                    IsActive = true
                                };
                                await _uow.OrderDetail.AddAsync(newOrder);
                            }
                            else
                            {
                                //Update
                                var order = current.OrderDetails.Where(p => p.Id == item.Id && p.IsActive).FirstOrDefault();
                                if (order != null && (order.Quantity != item.Quantity || order.Subtotal != item.SubTotal))
                                {
                                    order.Quantity = item.Quantity;
                                    order.Price = item.Price;
                                    order.Subtotal = item.SubTotal;
                                    _uow.OrderDetail.Update(order);
                                }
                            }
                        }
                    }
                    await _uow.SaveAsync();
                    await ExcuteLogAcitivity(current.Id, "Update", accId);
                    return ResponseBase.Result(true, 200);
                }
                return ResponseBase.Result(false, 404, "Lỗi hệ thống: Không tìm thấy dữ liệu");
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ResponseValue<IEnumerable<OrderViewModel>>> GetAllAsync()
        {
            try
            {
                string query = $"select O.Id, O.Code, O.Total, ISNULL(O.Note, '') as Note, sum(D.Quantity) as Quantity,O.Status, S.Name as [StatusName], O.CreatedOn from [Order] as O with(nolock) join L_Status as S with(nolock) on O.Status = S.Id join OrderDetail as D on D.OrderId = O.Id and D.IsActive = 1 group by O.Id, O.Code, O.Total,O.Note,O.Status, S.Name, O.CreatedOn ORDER BY O.CreatedOn DESC"; 
                var orders = await GetDapperDateAsync<OrderViewModel>(query);
                if (orders != null)
                {
                    return ResponseValue<IEnumerable<OrderViewModel>>.Result(true, 200, orders);
                }
                return ResponseValue<IEnumerable<OrderViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<OrderViewModel>>.Result(false, 500, null);
            }
        }

        public async Task<ResponseValue<OrderViewModel>> GetAsync(string orderId, string accId)
        {
            try
            {
                string condition = accId != string.Empty ? $"and (O.Status = 1 or O.CreatedBy = '{accId}' or O.ModifiedBy = '{accId}')" : "";
                string query = $"select O.Id, O.Code, sum(D.Quantity) as Quantity, O.Total, O.Status, S.Name as [StatusName], O.Note,O.ReasonCancel,O.CreatedOn, A.Username from [order] as O with(nolock) join L_Status as S with(nolock) on O.Status = S.Id join OrderDetail as D with(nolock) on O.Id = D.OrderId and D.IsActive = 1 join Account as A with(nolock) on A.Id = O.CreatedBy where O.Id = '{orderId}' {condition} group by O.Id, O.Code, O.Total, O.Status, S.Name, O.CreatedOn, O.ReasonCancel, O.Note, A.Username ORDER BY O.CreatedOn DESC";
                using (var conection = _dapper.CreateConnection())
                {
                    OrderViewModel order = await conection.QueryFirstAsync<OrderViewModel>(query);
                    if (order != null)
                    {
                        return ResponseValue<OrderViewModel>.Result(true, 200, order);
                    }
                    return ResponseValue<OrderViewModel>.Result(false, 404, null);
                }
            }
            catch (Exception ex)
            {
                return ResponseValue<OrderViewModel>.Result(false, 500, null);
            }
        }
        public async Task<ResponseValue<UpdateOrderViewModel>> GetUpdateAsync(string orderId)
        {
            try
            {
                string query = $"select O.Id, O.Code, Sum(S.Quantity) as Quantity,O.Total, ISNULL(O.Note, '') as Note, O.CreatedOn from [Order] as O with(nolock) join OrderDetail as S with(nolock) on O.Id = S.OrderId and S.IsActive = 1 where O.Id = '{orderId}' and O.Status = 1 group by O.Id, O.Total, O.Note, O.CreatedOn, O.Code";
                using (var conection = _dapper.CreateConnection())
                {
                    UpdateOrderViewModel model = new UpdateOrderViewModel();
                    model.Update = await conection.QueryFirstAsync<UpdateOrderAPIViewModel>(query);
                    if (model.Update != null)
                    {
                        return ResponseValue<UpdateOrderViewModel>.Result(true, 200, model); 
                    }
                    return ResponseValue<UpdateOrderViewModel>.Result(false, 404, null);
                }
            }
            catch (Exception ex)
            {
                return ResponseValue<UpdateOrderViewModel>.Result(false, 500, null);
            }
        }

        public async Task<ResponseBase> UpdateActiveAsync(CancelOrderViewModel model, string accId)
        {

            try
            {
                Order order = await _uow.Order.GetFirstOrDefaultAsync(p => p.Id == model.Id);
                if (order != null)
                {
                    order.Status = 3;
                    order.ReasonCancel = model.Resaon;
                    order.ModifiedOn = DateTime.Now;
                    order.ModifiedBy = accId;
                    _uow.Order.Update(order);
                    await _uow.SaveAsync();
                    await ExcuteLogAcitivity(order.Id, "Cancel", accId);
                    return ResponseBase.Result(true, 200);
                }
                return ResponseBase.Result(false, 404, "Lỗi hệ thống: Không tìm thấy dữ liệu");
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        private async Task<string> GenerateCode()
        {
            string currentDate = DateTime.Now.ToString("yyMMdd");
            var orders = await _uow.Order.GetAllAsync(p => p.Code.StartsWith($"#{currentDate}"));
            if (orders != null && orders.Any())
            {
                string maxCodeSuffix = orders.Max(p => p.Code.Substring(7, 3));

                if (int.TryParse(maxCodeSuffix, out int maxIndex))
                {
                    maxIndex += 1;
                }
                else
                {
                    maxIndex = 1;
                }

                return $"#{currentDate}{maxIndex.ToString("D3")}";
            }
            return $"#{currentDate}001";
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
        private async Task<IEnumerable<T>> GetDapperDateAsync<T>(string query)
        {
            using (var conection = _dapper.CreateConnection())
            {
                try
                {
                    var result = await conection.QueryAsync<T>(query);
                    return result.ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public async Task<ResponseValue<IEnumerable<OrderViewModel>>> GetAllOrderByShiftIdIsComplete(string shiftId)
        {
            try
            {
                string query = $"select O.Id, O.ShiftId,O.[Status],O.Total,sum(Od.Quantity) as Quantity, O.CreatedOn, O.Code from[Order] as O join OrderDetail as Od on O.Id = Od.OrderId where O.ShiftId like '{shiftId}' and O.[Status] = 2 group by  O.Id, O.ShiftId, O.[Status], O.Total, O.CreatedOn,O.Code";
                var orders = await GetDapperDateAsync<OrderViewModel>(query);
                if (orders != null)
                {
                    return ResponseValue<IEnumerable<OrderViewModel>>.Result(true, 200, orders);
                }
                return ResponseValue<IEnumerable<OrderViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<OrderViewModel>>.Result(false, 500, null);
            }
        }


        public async Task<ResponseValue<IEnumerable<OrderViewModel>>> GetAllInCompleteOrderAsync()
        {
            try
            {
                string query = $"select O.Id, O.ShiftId,O.[Status],O.Total, sum(Od.Quantity) as Quantity , O.CreatedOn, S.Name as [StatusName], O.Code, A.Username from [Order] as O join OrderDetail as Od with(nolock) on O.Id = Od.OrderId join L_Status as S with(nolock) on O.Status = S.Id join Account as A with(nolock) on A.Id = O.CreatedBy where O.[Status] = 1 group by  O.Id, O.ShiftId, O.[Status], O.Total, O.CreatedOn, S.Name, O.Code, A.Username ORDER BY O.CreatedOn DESC";
                var orders = await GetDapperDateAsync<OrderViewModel>(query);
                if (orders != null)
                {
                    return ResponseValue<IEnumerable<OrderViewModel>>.Result(true, 200, orders);
                }
                return ResponseValue<IEnumerable<OrderViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<OrderViewModel>>.Result(false, 500, null);
            }
        }

        public async Task<ResponseValue<IEnumerable<OrderViewModel>>> GetAllCompleteOrderAsync(string accId)
        {
            try
            {
                //string query = $"select O.Id, O.Code, O.Total, ISNULL(O.Note, '') as Note, sum(D.Quantity) as Quantity,O.Status, S.Name as [StatusName], O.CreatedOn from [Order] as O with(nolock) join L_Status as S with(nolock) on O.Status = S.Id join OrderDetail as D on D.OrderId = O.Id and D.IsActive = 1 where O.[Status] = 2 and CONVERT(date, O.CreatedOn) = CONVERT(date, GETDATE()) and O.CreatedBy like '{accId}' group by O.Id, O.Code, O.Total,O.Note,O.Status, S.Name, O.CreatedOn ORDER BY O.CreatedOn DESC";
                string query = $"select O.Id, O.Code, O.Total, ISNULL(O.Note, '') as Note, sum(D.Quantity) as Quantity,O.Status, S.Name as [StatusName], O.CreatedOn,O.ShiftId from [Order] as O with(nolock) join L_Status as S with(nolock) on O.Status = S.Id join OrderDetail as D on D.OrderId = O.Id and D.IsActive = 1 join Shift as Sh on O.ShiftId = Sh.Id where O.[Status] = 2 and O.CreatedBy like '{accId}' and Sh.TimeStart = (select MAX(Sh2.TimeStart) from Shift as Sh2 where CONVERT(date, Sh2.TimeStart) = CONVERT(date, GETDATE()))  group by O.Id, O.Code, O.Total,O.Note,O.Status, S.Name, O.CreatedOn,O.ShiftId ORDER BY O.CreatedOn DESC";
                var orders = await GetDapperDateAsync<OrderViewModel>(query);
                if (orders != null)
                {
                    return ResponseValue<IEnumerable<OrderViewModel>>.Result(true, 200, orders);
                }
                return ResponseValue<IEnumerable<OrderViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<OrderViewModel>>.Result(false, 500, null);
            }

        }

        public async Task<ResponseValue<IEnumerable<OrderViewModel>>> GetAllOrderCanceled(string accId)
        {
            try
            {
                string query = $"select O.Id, O.Code, O.Total, ISNULL(O.Note, '') as Note, sum(D.Quantity) as Quantity,O.Status, S.Name as [StatusName], O.CreatedOn,O.ShiftId from [Order] as O with(nolock) join L_Status as S with(nolock) on O.Status = S.Id join OrderDetail as D on D.OrderId = O.Id and D.IsActive = 1 join Shift as Sh on O.ShiftId = Sh.Id where O.[Status] = 3 and O.CreatedBy like '{accId}' and Sh.TimeStart = (select MAX(Sh2.TimeStart) from Shift as Sh2 where CONVERT(date, Sh2.TimeStart) = CONVERT(date, GETDATE()))  group by O.Id, O.Code, O.Total,O.Note,O.Status, S.Name, O.CreatedOn,O.ShiftId ORDER BY O.CreatedOn DESC";
                var orders = await GetDapperDateAsync<OrderViewModel>(query);
                if (orders != null)
                {
                    return ResponseValue<IEnumerable<OrderViewModel>>.Result(true, 200, orders);
                }
                return ResponseValue<IEnumerable<OrderViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<OrderViewModel>>.Result(false, 500, null);
            }
        }
    }
}
