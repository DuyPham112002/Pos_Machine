using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Services.AttributeSetService
{
    public interface IAttributeSetService
    {
        Task<ResponseBase> CreateAsync(string attributeSetId, string accId);
    }
    public class AttributeSetService : IAttributeSetService
    {
        private readonly IUnitOfWork _uow;
        public AttributeSetService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<ResponseBase> CreateAsync(string attributeSetId, string accId)
        {
            try
            {
                AttributeSet attributeSet = new AttributeSet
                {
                    Id = attributeSetId,
                    IsActive = true
                };
                await _uow.AttributeSet.AddAsync(attributeSet);
                await _uow.SaveAsync();
                return ResponseBase.Result(true, 200);
            }
            catch (Exception ex) {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }
    }
}
