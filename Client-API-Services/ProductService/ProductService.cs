using Client_DBAccess.Dapper;
using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Attribute;
using Client_ViewModel.Category;
using Client_ViewModel.Product;
using Client_ViewModel.Response;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Services.ProductService
{
    public interface IProductService
    {
        Task<ResponseBase> CreateAsync(CreateProductAPIViewModel model, string accId);
        Task<ResponseBase> UpdateAsync(UpdateProductViewModel model, string accId);
        Task<ResponseValue<ProductViewModel>> GetAsync(string productId, string accId);
        Task<ResponseValue<IEnumerable<ProductViewModel>>> GetAllAsync(string? active = null);
        Task<ResponseValue<IEnumerable<ProductViewModel>>> GetAllBySubCategoryAsync(string subcategoryId, string? active = null);
        Task<ResponseBase> UpdateActiveAsync(string productId, string accId);
    }
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDapperContext _dapper;
        public ProductService(IUnitOfWork uow, IDapperContext dapper)
        {
            _uow = uow;
            _dapper = dapper;
        }
        public async Task<ResponseBase> CreateAsync(CreateProductAPIViewModel model, string accId)
        {
            try
            {
                Product product = await _uow.Product.GetFirstOrDefaultAsync(p => p.Name == model.Name && p.SubCategoryId == model.SubCategoryId);
                if (product == null)
                {
                    Product newProduct = new Product
                    {
                        Id = Guid.NewGuid().ToString(),
                        SubCategoryId = model.SubCategoryId,
                        AttributeSetId = model.AttributeSetId,
                        ImgSetId = model.ImgSetId,
                        Name = model.Name,
                        Price = Convert.ToDouble(model.Price.Replace("VNĐ", "")
                                   .Replace(".", "")
                                   .Replace(",", "")
                                   .Trim()),
                        IsRequiredAttribute = model.IsRequiredAttribute == 1 ? false : true,
                        Description = model.Description,
                        IsActive = true,
                        CreatedOn = DateTime.Now,
                        CreatedBy = accId
                    };
                    await _uow.Product.AddAsync(newProduct);
                    await _uow.SaveAsync();
                    return ResponseBase.Result(true, 200);
                }
                return ResponseBase.Result(false, 400, "Sản phẩm đã tồn tại trong hệ thống");
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ResponseValue<IEnumerable<ProductViewModel>>> GetAllAsync(string? active = null)
        {
            try
            {
                string conditon = !string.IsNullOrEmpty(active) && active == "Active" ? "and C.IsActive = 1" : string.Empty;
                string query = $"select P.Id, C.Id as [CategoryId], c.Name as [CategoryName], P.SubCategoryId, S.Name as [SubCategoryName], P.Name, P.Price, P.Description, P.IsActive, P.ImgSetId as [ImageSetId], I.ImageUrl as [images], P.CreatedOn, M.Fullname as [CreatedBy] from Product as P with(nolock) join SubCategory as S with(nolock) on P.SubCategoryId = S.Id join Category as C with(nolock) on C.Id = S.CategoryId join Manager as M with(nolock) on P.CreatedBy = M.AccId join Image as I with(nolock) on I.ImgSetId = P.ImgSetId and I.IsActive = 1 {conditon}";
                var products = await GetDapperDateAsync<ProductViewModel>(query);
                if (products != null)
                {
                    return ResponseValue<IEnumerable<ProductViewModel>>.Result(true, 200, products);
                }
                return ResponseValue<IEnumerable<ProductViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<ProductViewModel>>.Result(false, 500, null);
            }
        }

        public async Task<ResponseValue<IEnumerable<ProductViewModel>>> GetAllBySubCategoryAsync(string subcategoryId, string? active = null)
        {
            try
            {
                string conditon = !string.IsNullOrEmpty(active) && active == "Active" ? "and C.IsActive = 1 and P.IsActive = 1" : string.Empty;
                string query = $"select P.Id, cast(case when P.IsRequiredAttribute = 1 then 2 else 1 end as int) as IsRequiredAttribute, C.Id as [CategoryId], c.Name as [CategoryName], P.SubCategoryId, S.Name as [SubCategoryName], P.Name, P.Price, P.Description, P.IsActive, P.ImgSetId as [ImageSetId], I.ImageUrl as [images], P.CreatedOn, M.Fullname as [CreatedBy] from Product as P with(nolock) join SubCategory as S with(nolock) on P.SubCategoryId = S.Id join Category as C with(nolock) on C.Id = S.CategoryId join Manager as M with(nolock) on P.CreatedBy = M.AccId join Image as I with(nolock) on I.ImgSetId = P.ImgSetId and I.IsActive = 1 where SubCategoryId = '{subcategoryId}' {conditon} order by P.CreatedOn DESC";
                var products = await GetDapperDateAsync<ProductViewModel>(query);
                if (products != null)
                {
                    return ResponseValue<IEnumerable<ProductViewModel>>.Result(true, 200, products);
                }
                return ResponseValue<IEnumerable<ProductViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<ProductViewModel>>.Result(false, 500, null);
            }
        }

        public async Task<ResponseValue<ProductViewModel>> GetAsync(string productId, string accId)
        {
            try
            {
                string condition = accId != string.Empty ? "and P.IsActive = 1" : "";
                string query = $"select P.Id, C.Id as [CategoryId], c.Name as [CategoryName], P.SubCategoryId, S.Name as [SubCategoryName], P.Name, P.Price, P.Description, P.IsActive, P.ImgSetId as [ImageSetId], P.AttributeSetId, I.ImageUrl as [images], P.CreatedOn, M.Fullname as [CreatedBy], cast(case when P.IsRequiredAttribute = 1 then 2 else 1 end as int) as IsRequiredAttribute from Product as P with(nolock) join SubCategory as S with(nolock) on P.SubCategoryId = S.Id join Category as C with(nolock) on C.Id = S.CategoryId join Manager as M with(nolock) on P.CreatedBy = M.AccId join Image as I with(nolock) on I.ImgSetId = P.ImgSetId and I.IsActive = 1 where P.Id = '{productId}' {condition}";
                using (var conection = _dapper.CreateConnection())
                {
                    var product = await conection.QueryFirstAsync<ProductViewModel>(query);
                    if (product != null)
                    {
                        return ResponseValue<ProductViewModel>.Result(true, 200, product);
                    }
                    return ResponseValue<ProductViewModel>.Result(false, 404, null);
                }
            }
            catch (Exception ex)
            {
                return ResponseValue<ProductViewModel>.Result(false, 500, null);
            }
        }

        public async Task<ResponseBase> UpdateAsync(UpdateProductViewModel model, string accId)
        {
            try
            {
                Product product = await _uow.Product.GetFirstOrDefaultAsync(p => p.Id == model.Id);
                if (product == null)
                    return ResponseBase.Result(false, 404, $"Lỗi hệ thống: Không tìm thấy dữ liệu");
                bool exist = false;
                if (product.Name != model.Name || product.SubCategoryId != model.SubCategoryId)
                    exist = (await _uow.Product.GetFirstOrDefaultAsync(p => p.Name == model.Name && p.SubCategoryId == model.SubCategoryId)) != null;
                if (!exist)
                {
                    product.Name = model.Name;
                    product.Price = Convert.ToDouble(model.Price.Replace("VNĐ", "")
                                   .Replace(".", "")
                                   .Replace(",", "")
                                   .Trim());
                    product.IsRequiredAttribute = model.IsRequiredAttribute == 1 ? false : true;
                    product.SubCategoryId = model.SubCategoryId;
                    product.ModifiedBy = accId;
                    product.Description = model.Description;
                    product.ModifiedOn = DateTime.Now;
                    _uow.Product.Update(product);
                    await _uow.SaveAsync();
                    return ResponseBase.Result(true, 200);
                }
                return ResponseBase.Result(false, 400, "Sản phẩm đã tồn tại trong hệ thống");
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ResponseBase> UpdateActiveAsync(string productId, string accId)
        {

            try
            {
                Product product = await _uow.Product.GetFirstOrDefaultAsync(p => p.Id == productId);
                if (product != null)
                {
                    product.IsActive = !product.IsActive;
                    product.ModifiedOn = DateTime.Now;
                    product.ModifiedBy = accId;
                    _uow.Product.Update(product);
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
