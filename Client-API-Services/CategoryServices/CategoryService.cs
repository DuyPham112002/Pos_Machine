using Client_DBAccess.Dapper;
using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Category;
using Client_ViewModel.Employee;
using Client_ViewModel.Manager;
using Client_ViewModel.Response;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<ResponseBase> CreateAsync(CreateCategoryViewModel model, string accId);
        Task<ResponseBase> UpdateAsync(UpdateCategoryViewModel model, string accId);
        Task<ResponseValue<CategoryViewModel>> GetAsync(string categoryId);
        Task<ResponseValue<IEnumerable<CategoryViewModel>>> GetAllAsync(string? active = null);
        Task<ResponseBase> UpdateActiveAsync(string categoryId, string accId);
    }
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDapperContext _dapper;
        public CategoryService(IUnitOfWork uow, IDapperContext dapper)
        {
            _uow = uow;
            _dapper = dapper;
        }
        public async Task<ResponseBase> CreateAsync(CreateCategoryViewModel model, string accId)
        {
            try
            {
                Category category = await _uow.Category.GetFirstOrDefaultAsync(p => p.Name == model.Name);
                if (category == null)
                {
                    Category newCategory = new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = model.Name,
                        IsActive = true,
                        CreatedOn = DateTime.Now,
                        CreatedBy = accId
                    };
                    await _uow.Category.AddAsync(newCategory);
                    await _uow.SaveAsync();
                    return ResponseBase.Result(true, 200);
                }
                return ResponseBase.Result(false, 400, "Danh mục đã tồn tại trong hệ thống");
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ResponseBase> UpdateAsync(UpdateCategoryViewModel model, string accId)
        {
            try
            {
                Category category = await _uow.Category.GetFirstOrDefaultAsync(p => p.Id == model.Id);
                if (category == null)
                    return ResponseBase.Result(false, 404, $"Lỗi hệ thống: Không tìm thấy dữ liệu");
                bool exist = false;
                if (category.Name != model.Name)
                    exist = (await _uow.Category.GetFirstOrDefaultAsync(p => p.Name == model.Name)) != null;
                if (!exist)
                {
                    category.Name = model.Name;
                    category.ModifiedOn = DateTime.Now;
                    category.ModifiedBy = accId;
                    _uow.Category.Update(category);
                    await _uow.SaveAsync();
                    return ResponseBase.Result(true, 200);
                }
                return ResponseBase.Result(false, 400, "Danh mục đã tồn tại trong hệ thống");
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ResponseBase> UpdateActiveAsync(string categoryId, string accId)
        {
            try
            {
                Category category = await _uow.Category.GetFirstOrDefaultAsync(p => p.Id == categoryId, "SubCategories");
                if (category != null)
                {
                    category.IsActive = !category.IsActive;
                    if (!category.IsActive)
                        if (category.SubCategories.Any(p => p.IsActive))
                            return ResponseBase.Result(false, 400, "Không thể vô hiệu hóa danh mục này do các loại sản phẩm vẫn đang hoạt động");
                    category.ModifiedOn = DateTime.Now;
                    category.ModifiedBy = accId;
                    _uow.Category.Update(category);
                    await _uow.SaveAsync();
                    return ResponseBase.Result(true, 200);
                }
                return ResponseBase.Result(false, 404, "Lỗi hệ thống: Không tìm thấy dữ liệu");
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ResponseValue<CategoryViewModel>> GetAsync(string categoryId)
        {
            try
            {
                string query = $"SELECT C.Id, C.Name, C.IsActive, C.CreatedOn, M.Fullname AS CreatedBy FROM Category AS C WITH (NOLOCK) JOIN Manager AS M WITH (NOLOCK) ON C.CreatedBy = M.AccId where C.Id = '{categoryId}'";
                using (var conection = _dapper.CreateConnection())
                {
                    var category = await conection.QueryFirstAsync<CategoryViewModel>(query);
                    if (category != null)
                    {
                        return ResponseValue<CategoryViewModel>.Result(true, 200, category);
                    }
                    return ResponseValue<CategoryViewModel>.Result(false, 404, null);
                }
            }
            catch (Exception ex)
            {
                return ResponseValue<CategoryViewModel>.Result(false, 500, null);
            }
        }

        public async Task<ResponseValue<IEnumerable<CategoryViewModel>>> GetAllAsync(string? active = null)
        {
            try
            {
                string conditon = !string.IsNullOrEmpty(active) && active == "Active" ? "where C.IsActive = 1" : string.Empty;
                string query = $"select C.Id, C.Name, C.IsActive, C.CreatedOn, M.Fullname as [CreatedBy] from Category as C with(nolock) join Manager as M with(nolock) on C.CreatedBy = M.AccId {conditon}";
                var categories = await GetDapperDateAsync<CategoryViewModel>(query);
                if (categories != null)
                {
                    return ResponseValue<IEnumerable<CategoryViewModel>>.Result(true, 200, categories);
                }
                return ResponseValue<IEnumerable<CategoryViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<CategoryViewModel>>.Result(false, 500, null);
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
