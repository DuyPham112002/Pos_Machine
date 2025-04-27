using Client_DBAccess.Dapper;
using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Attribute;
using Client_ViewModel.Category;
using Client_ViewModel.Product;
using Client_ViewModel.Response;
using Dapper;
using System.Reflection;
using System.Text;

namespace Client_API_Services.AttributeService
{
    public interface IAttributeService
    {
        Task<ResponseBase> CreateAsync(string attributeSetId, List<CreateAttributeAPIViewModel> creates, string accId);
        Task<ResponseBase> UpdateAsync(string attributeSetId, List<UpdateAttributeAPIViewModel> updates, string accId);
        Task<ResponseValue<IEnumerable<AttributeViewModel>>> GetByProductAsync(string productId);
        Task<ResponseBase> SoftDeleteAsync(string attributeId, string accId);
    }
    public class AttributeService : IAttributeService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDapperContext _dapper;
        public AttributeService(IUnitOfWork uow, IDapperContext dapper)
        {
            _uow = uow;
            _dapper = dapper;
        }

        public async Task<ResponseBase> CreateAsync(string attributeSetId, List<CreateAttributeAPIViewModel> creates, string accId)
        {
            try
            {
                foreach (var item in creates)
                {
                    double priceConvert = Convert.ToDouble(item.Price.Replace("VNĐ", "")
                                   .Replace(".", "")
                                   .Replace(",", "")
                                   .Trim());
                    var current = await _uow.Attribute.GetFirstOrDefaultAsync(p => p.Name == item.Name && p.Price == priceConvert && p.AttributeSetId == attributeSetId);
                    if (current == null)
                    {
                        var newAttribute = new Client_DBAccess.Entities.Attribute
                        {
                            Id = Guid.NewGuid().ToString(),
                            AttributeSetId = attributeSetId,
                            Name = item.Name,
                            Price = priceConvert,
                            IsActive = true,
                            CreatedBy = accId,
                            CreatedOn = DateTime.Now,
                        };
                        await _uow.Attribute.AddAsync(newAttribute);
                    }
                    else
                    {
                        current.IsActive = true;
                        _uow.Attribute.Update(current);
                    }
                }
                await _uow.SaveAsync();
                return ResponseBase.Result(true, 200);
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ResponseValue<IEnumerable<AttributeViewModel>>> GetByProductAsync(string productId)
        {
            try
            {
                string query = $"select A.Id, A.Name, A.Price from Product as P join Attribute as A on P.AttributeSetId = A.AttributeSetId and A.IsActive = 1 where P.Id = '{productId}' order by A.Price Asc";
                var attributes = await GetDapperDateAsync<AttributeViewModel>(query);
                if (attributes != null)
                {
                    return ResponseValue<IEnumerable<AttributeViewModel>>.Result(true, 200, attributes);
                }
                return ResponseValue<IEnumerable<AttributeViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<AttributeViewModel>>.Result(false, 500, null);
            }
        }

        public async Task<ResponseBase> SoftDeleteAsync(string attributeId, string accId)
        {
            try
            {
                var attribute = await _uow.Attribute.GetFirstOrDefaultAsync(p => p.Id == attributeId);
                if (attribute != null)
                {
                    attribute.IsActive = false;
                    attribute.ModifiedOn = DateTime.UtcNow;
                    attribute.ModifiedBy = accId;
                    _uow.Attribute.Update(attribute);
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

        public async Task<ResponseBase> UpdateAsync(string attributeSetId, List<UpdateAttributeAPIViewModel> updates, string accId)
        {
            try
            {
                AttributeSet attributeSet = await _uow.AttributeSet.GetFirstOrDefaultAsync(p => p.Id == attributeSetId, "Attributes");
                if (attributeSet != null)
                {
                    var notExists = attributeSet.Attributes
                                      .Where(p => !updates.Any(o => o.Id == p.Id) && p.IsActive)
                                      .ToList();
                    foreach (var item in updates)
                    {
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            //Create
                            var newAttribute = new Client_DBAccess.Entities.Attribute
                            {
                                Id = Guid.NewGuid().ToString(),
                                AttributeSetId = attributeSetId,
                                Name = item.Name,
                                Price = Convert.ToDouble(item.Price.Replace("VNĐ", "")
                                   .Replace(".", "")
                                   .Replace(",", "")
                                   .Trim()),
                                IsActive = true,
                                CreatedBy = accId,
                                CreatedOn = DateTime.Now,
                            };
                            await _uow.Attribute.AddAsync(newAttribute);
                        }
                        else
                        {
                            var attribute = attributeSet.Attributes.Where(p => p.Id == item.Id && p.IsActive).FirstOrDefault();
                            if (attribute != null)
                            {
                                //Update
                                attribute.Name = item.Name;
                                attribute.Price = Convert.ToDouble(item.Price.Replace("VNĐ", "")
                                       .Replace(".", "")
                                       .Replace(",", "")
                                       .Trim());
                                attribute.ModifiedOn = DateTime.Now;
                                attribute.ModifiedBy = accId;
                                _uow.Attribute.Update(attribute);
                            }
                        }
                    }

                    //Delete
                    if (notExists != null && notExists.Count > 0)
                    {
                        foreach (var attribute in notExists) {
                            attribute.IsActive = false;
                            _uow.Attribute.Update(attribute);
                        }
                    }

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
