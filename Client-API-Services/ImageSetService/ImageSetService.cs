using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Services.ImageSetService
{
    public interface IImageSetService
    {
        Task<ResponseBase> CreateAsync(string imgSetId);
    }
    public class ImageSetService : IImageSetService
    {
        private readonly IUnitOfWork _uow;
        public ImageSetService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ResponseBase> CreateAsync(string imgSetId)
        {
            try
            {
                var newImgSet = new ImgSet
                {
                    Id = imgSetId,
                    IsActive = true
                };
                await _uow.ImgSet.AddAsync(newImgSet);
                await _uow.SaveAsync();
                return ResponseBase.Result(true, 200);
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(true, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }
    }
}
