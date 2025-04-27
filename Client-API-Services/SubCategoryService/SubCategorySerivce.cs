using Client_DBAccess.Dapper;
using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Category;
using Client_ViewModel.Response;
using Client_ViewModel.SubCategory;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Services.SubCategoryService
{
    public interface ISubCategorySerivce
    {
        Task<ResponseBase> CreateAsync(CreateSubCategoryViewModel model, string accId);
        Task<ResponseBase> UpdateAsync(UpdateSubCategoryViewModel model, string accId);
        Task<ResponseValue<SubCategoryViewModel>> GetAsync(string subCategoryId);
        Task<ResponseValue<IEnumerable<SubCategoryViewModel>>> GetAllByCategoryAsync(string categoryId, string? active = null);
        Task<ResponseValue<IEnumerable<SubCategoryViewModel>>> GetAllAsync();
        Task<ResponseBase> UpdateActiveAsync(string subCategoryId, string accId);
    }
    public class SubCategorySerivce : ISubCategorySerivce
    {
        private readonly IUnitOfWork _uow;
        private readonly IDapperContext _dapper;
        public SubCategorySerivce(IUnitOfWork uow, IDapperContext dapper)
        {
            _uow = uow; 
            _dapper = dapper;
        }
        public async Task<ResponseBase> CreateAsync(CreateSubCategoryViewModel model, string accId)
        {
            try
            {
                SubCategory subCategory = await _uow.SubCategory.GetFirstOrDefaultAsync(p => p.Name == model.Name && p.CategoryId == model.CategoryId);
                if (subCategory == null)
                {
                    SubCategory newSubCategory = new SubCategory
                    {
                        Id = Guid.NewGuid().ToString(),
                        CategoryId = model.CategoryId,
                        Name = model.Name,
                        IsActive = true,
                        CreatedOn = DateTime.Now,
                        CreatedBy = accId
                    };
                    await _uow.SubCategory.AddAsync(newSubCategory);
                    await _uow.SaveAsync();
                    return ResponseBase.Result(true, 200);
                }
                return ResponseBase.Result(false, 400, "loại sản phẩm đã tồn tại trong hệ thống");
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ResponseValue<IEnumerable<SubCategoryViewModel>>> GetAllAsync()
        {
            try
            {
                string query = $"select C.Id, C.Name, S.Id as [CategoryId], S.Name as [CategoryName],C.IsActive, C.CreatedOn, M.Fullname as [CreatedBy] from SubCategory as C with(nolock) join Category as S with(nolock) on C.CategoryId = S.Id join Manager as M with(nolock) on C.CreatedBy = M.AccId";
                var subCategories = await GetDapperDateAsync<SubCategoryViewModel>(query);
                if (subCategories != null)
                {
                    return ResponseValue<IEnumerable<SubCategoryViewModel>>.Result(true, 200, subCategories);
                }
                return ResponseValue<IEnumerable<SubCategoryViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<SubCategoryViewModel>>.Result(false, 500, null);
            }
        }

        public async Task<ResponseValue<IEnumerable<SubCategoryViewModel>>> GetAllByCategoryAsync(string categoryId, string? active = null)
        {
            try
            {
                string conditon = !string.IsNullOrEmpty(active) && active == "Active" ? "and C.IsActive = 1" : string.Empty;
                string query = $"select C.Id, C.Name, S.Id as [CategoryId], S.Name as [CategoryName],C.IsActive, C.CreatedOn, M.Fullname as [CreatedBy] from SubCategory as C with(nolock) join Category as S with(nolock) on C.CategoryId = S.Id join Manager as M with(nolock) on C.CreatedBy = M.AccId where c.CategoryId = '{categoryId}' {conditon}";
                var subCategories = await GetDapperDateAsync<SubCategoryViewModel>(query);
                if (subCategories != null)
                {
                    return ResponseValue<IEnumerable<SubCategoryViewModel>>.Result(true, 200, subCategories);
                }
                return ResponseValue<IEnumerable<SubCategoryViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<SubCategoryViewModel>>.Result(false, 500, null);
            }
        }

        public async Task<ResponseValue<SubCategoryViewModel>> GetAsync(string subCategoryId)
        {
            try
            {
                string query = $"select C.Id, C.Name, S.Id as [CategoryId], S.Name as [CategoryName],C.IsActive, C.CreatedOn, M.Fullname as [CreatedBy] from SubCategory as C with(nolock) join Category as S with(nolock) on C.CategoryId = S.Id join Manager as M with(nolock) on C.CreatedBy = M.AccId where C.Id = '{subCategoryId}'";
                using (var conection = _dapper.CreateConnection())
                {
                    var subCategory = await conection.QueryFirstAsync<SubCategoryViewModel>(query);
                    if (subCategory != null)
                    {
                        return ResponseValue<SubCategoryViewModel>.Result(true, 200, subCategory);
                    }
                    return ResponseValue<SubCategoryViewModel>.Result(false, 404, null);
                }
            }
            catch (Exception ex)
            {
                return ResponseValue<SubCategoryViewModel>.Result(false, 500, null);
            }
        }

        public async Task<ResponseBase> UpdateActiveAsync(string subCategoryId, string accId)
        {
            try
            {
                SubCategory subCategory = await _uow.SubCategory.GetFirstOrDefaultAsync(p => p.Id == subCategoryId, "Products");
                if (subCategory != null)
                {
                    subCategory.IsActive = !subCategory.IsActive;
                    if (!subCategory.IsActive)
                        if (subCategory.Products.Any(p => p.IsActive))
                            return ResponseBase.Result(false, 400, "Không thể vô hiệu hóa loại sản phẩm này do các sản phẩm vẫn đang hoạt động");
                    subCategory.ModifiedOn = DateTime.Now;
                    subCategory.ModifiedBy = accId;
                    _uow.SubCategory.Update(subCategory);
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

        public async Task<ResponseBase> UpdateAsync(UpdateSubCategoryViewModel model, string accId)
        {
            try
            {
                SubCategory subCategory = await _uow.SubCategory.GetFirstOrDefaultAsync(p => p.Id == model.Id);
                if (subCategory == null)
                    return ResponseBase.Result(false, 404, $"Lỗi hệ thống: Không tìm thấy dữ liệu");
                bool exist = false;
                if (subCategory.Name != model.Name || subCategory.CategoryId != model.CategoryId)
                    exist = (await _uow.SubCategory.GetFirstOrDefaultAsync(p => p.Name == model.Name && p.CategoryId == model.CategoryId)) != null;
                if (!exist)
                {
                    subCategory.CategoryId = model.CategoryId;
                    subCategory.Name = model.Name;
                    subCategory.ModifiedOn = DateTime.Now;
                    subCategory.ModifiedBy = accId;
                    _uow.SubCategory.Update(subCategory);
                    await _uow.SaveAsync();
                    return ResponseBase.Result(true, 200);
                }
                return ResponseBase.Result(false, 400, "loại sản phẩm đã tồn tại trong hệ thống");
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
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
