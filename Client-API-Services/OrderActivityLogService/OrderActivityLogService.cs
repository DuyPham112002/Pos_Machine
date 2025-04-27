using Client_DBAccess.Dapper;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.OrderActivityLog;
using Client_ViewModel.OrderDetail;
using Client_ViewModel.Response;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Services.OrderActivityLogService
{
    public interface IOrderActivityLogService
    {
        Task<IEnumerable<ActivityLogViewModel>> GetActivityLogAsync(string orderId);
    }
    public class OrderActivityLogService : IOrderActivityLogService
    {
        private readonly IDapperContext _dapper;
        public OrderActivityLogService(IDapperContext dapper)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<ActivityLogViewModel>> GetActivityLogAsync(string orderId)
        {
            try
            {
                string query = $"select L.Id, L.LogActivated, L.CreatedOn, E.Fullname as CreatedBy from OrderActivityLog as L with(nolock) join Employee as E with(nolock) on L.CreatedBy = E.AccId where L.OrderId = '{orderId}' order by L.CreatedOn desc";
                var logs = await GetDapperDateAsync<ActivityLogViewModel>(query);
                if (logs != null)
                {
                    return logs;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
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
