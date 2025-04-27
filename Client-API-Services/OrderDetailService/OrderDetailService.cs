using Client_DBAccess.Dapper;
using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Order;
using Client_ViewModel.OrderDetail;
using Client_ViewModel.Response;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Client_API_Services.OrderDetailService
{
    public interface IOrderDetailService
    {
        Task<ResponseBase> CreateAsync(List<CreateOrderDetailViewModel> creates, string orderId, string accId);
        Task<ResponseValue<IEnumerable<OrderDetailViewModel>>> GetAllAsync(string orderId);
        Task<ResponseValue<IEnumerable<OrderDetailViewModel>>> GetDetailByOrderAsync(string orderId);
        Task<ResponseValue<IEnumerable<CreateOrderDetailViewModel>>> GetUpdateByOrderAsync(string orderId);
    }
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDapperContext _dapper;
        public OrderDetailService(IUnitOfWork uow, IDapperContext dapper)
        {
            _uow = uow;
            _dapper = dapper;
        }

        public async Task<ResponseBase> CreateAsync(List<CreateOrderDetailViewModel> creates, string orderId, string accId)
        {
            try
            {
                foreach (var item in creates)
                {
                    OrderDetail order = new OrderDetail
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = orderId,
                        ProductId = item.ProductId,
                        AttributeId = !string.IsNullOrEmpty(item.AttributeId) ? item.AttributeId : null,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Subtotal = item.Price * item.Quantity, 
                        IsActive = true,
                        CreatedOn = DateTime.Now,
                        CreatedBy = accId
                    };
                    await _uow.OrderDetail.AddAsync(order);
                }
                await _uow.SaveAsync();
                return ResponseBase.Result(true, 200);
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ResponseValue<IEnumerable<OrderDetailViewModel>>> GetAllAsync(string orderId)
        {
            try
            {
                string query = $"select D.Id, D.ProductId, I.ImageUrl, D.Price, D.Quantity, D.Subtotal, ISNULL(D.Note, '') as Note, case when A.AttributeSetId is not null then CONCAT(P.Name, ' <br><span>(', A.Name,')</span>') else p.Name end  as [ProductName] from OrderDetail as D with(nolock) left join Attribute as A with(nolock) on A.Id = D.AttributeId join Product as P with(nolock) on D.ProductId = P.Id join Image as I with(nolock) on I.ImgSetId = P.ImgSetId and I.IsActive = 1 where D.OrderId = '{orderId}' and D.IsActive = 1";
                var orders = await GetDapperDateAsync<OrderDetailViewModel>(query);
                if (orders != null)
                {
                    return ResponseValue<IEnumerable<OrderDetailViewModel>>.Result(true, 200, orders);
                }
                return ResponseValue<IEnumerable<OrderDetailViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<OrderDetailViewModel>>.Result(false, 500, null);
            }
        }

        public async Task<ResponseValue<IEnumerable<OrderDetailViewModel>>> GetDetailByOrderAsync(string orderId)
        {
            try
            {
                string query = $"select D.Id, D.ProductId, A.Id as AttributeId, case when A.AttributeSetId is not null then CONCAT(P.Name, ' <br><span>(', A.Name,')</span>') else p.Name end  as [ProductName], I.ImageUrl, d.Price, d.Quantity, d.Subtotal, d.CreatedOn from [Order] as O with(nolock) join OrderDetail as D with(nolock) on O.Id = D.OrderId and D.IsActive = 1 join Product as P with(nolock) on D.ProductId = P.Id join Image as I with(nolock) on I.ImgSetId = P.ImgSetId and I.IsActive = 1 left join Attribute as A on D.AttributeId = A.Id where O.Id = '{orderId}' order by P.Id asc";
                var orders = await GetDapperDateAsync<OrderDetailViewModel>(query);
                if (orders != null)
                {
                    return ResponseValue<IEnumerable<OrderDetailViewModel>>.Result(true, 200, orders);
                }
                return ResponseValue<IEnumerable<OrderDetailViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<OrderDetailViewModel>>.Result(false, 500, null);
            }
        }

        public async Task<ResponseValue<IEnumerable<CreateOrderDetailViewModel>>> GetUpdateByOrderAsync(string orderId)
        {
            try
            {
                string query = $"select O.Id, O.ProductId, A.Id as AttributeId, case when A.AttributeSetId is not null then CONCAT(P.Name, ' <br><span>(', A.Name,')</span>') else p.Name end  as [Name], O.Quantity, O.Price, O.Subtotal from OrderDetail as O with(nolock) join Product as P with(nolock) on O.ProductId = P.Id left join Attribute as A on A.Id = O.AttributeId where O.OrderId = '{orderId}' and O.IsActive = 1 order by P.Id asc";
                var orders = await GetDapperDateAsync<CreateOrderDetailViewModel>(query);
                if (orders != null)
                {
                    return ResponseValue<IEnumerable<CreateOrderDetailViewModel>>.Result(true, 200, orders);
                }
                return ResponseValue<IEnumerable<CreateOrderDetailViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<CreateOrderDetailViewModel>>.Result(false, 500, null);
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
    }
}
