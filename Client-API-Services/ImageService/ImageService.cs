using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Image;
using Client_ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Services.ImageService
{
    public interface IImageService
    {
        Task<bool> CreateManagerImageAsync(string contentRoot, ImageViewModel image);
        Task<bool> CreateEmpoyeeImageAsync(string contentRoot, ImageViewModel image);
        Task<bool> CreateProductImageAsync(string contentRoot, ImageViewModel image);
    }
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _uow;
        public ImageService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> CreateManagerImageAsync(string contentRoot, ImageViewModel image)
        {
            return await CreateImageBase(contentRoot, image);
        }

        public async Task<bool> CreateEmpoyeeImageAsync(string contentRoot, ImageViewModel image)
        {
            return await CreateImageBase(contentRoot, image);
        }

        public async Task<bool> CreateProductImageAsync(string contentRoot, ImageViewModel image)
        {
            return await CreateImageBase(contentRoot, image);
        }

        private async Task<bool> CreateImageBase(string contentRoot, ImageViewModel image)
        {
            try
            {
                var images = await _uow.Image.GetAllAsync(p => p.ImgSetId == image.ImgSetId && p.IsActive);
                if (images != null && images.Any())
                {
                    foreach (var item in images)
                    {
                        item.IsActive = false;
                        var filePath = contentRoot + "\\wwwroot\\" + item.ImageUrl;
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }
                }
                Image newImage = new Image()
                {
                    Id = Guid.NewGuid().ToString(),
                    ImgSetId = image.ImgSetId,
                    ImageUrl = image.ImgUrl,
                    IsActive = true
                };
                await _uow.Image.AddAsync(newImage);
                await _uow.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
