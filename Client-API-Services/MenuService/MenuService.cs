using Client_DBAccess.Dapper;
using Client_ViewModel.Menu;
using Client_ViewModel.OrderDetail;
using Client_ViewModel.Response;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Services.MenuService
{
    public interface IMenuService
    {
        Task<ResponseValue<IEnumerable<MenuViewModel>>> GetAllAsync();
    }
    public class MenuService : IMenuService
    {
        private readonly IDapperContext _dapper;
        public MenuService(IDapperContext dapper)
        {
            _dapper = dapper;
        }

        public async Task<ResponseValue<IEnumerable<MenuViewModel>>> GetAllAsync()
        {
            try
            {
                string query = $"select C.Name as [CategoryName], ISNULL(S.Name, '') as [SubcategoryName], P.Id as [ProductId], ISNULL(P.Name, '') as [ProductName] from Category as C with(nolock) join SubCategory as S with(nolock) on C.Id = S.CategoryId and S.IsActive = 1 join Product as P with(nolock) on S.Id = P.SubCategoryId and P.IsActive = 1 where C.IsActive = 1 order by C.CreatedOn asc";
                var menus = await GetDapperDateAsync<MenuViewModel>(query);
                if (menus != null)
                {
                    return ResponseValue<IEnumerable<MenuViewModel>>.Result(true, 200, menus);
                }
                return ResponseValue<IEnumerable<MenuViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<MenuViewModel>>.Result(false, 500, null);
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
