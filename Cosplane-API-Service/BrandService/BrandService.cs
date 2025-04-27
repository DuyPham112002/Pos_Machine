using Cosplane_API_DBAccess.Dapper;
using Cosplane_API_DBAccess.Entities;
using Cosplane_API_DBAccess.UnitOfWork;
using Cosplane_API_ViewModel.Brand;
using Dapper;

namespace Cosplane_API_Service.BrandService
{
    public interface IBrandService
    {
        Task<int> CreateBrandAsync(CreateBrandAPIViewModel info, string creatorId);
        Task<List<GetAllBrandAPIViewModel>> GetAllAsync();
        Task<bool> UpdateByBrandIdAsync(UpdateBrandAPIViewModel newBrand);
        Task<bool> DeleteByBrandIdAsync(string brandId);
        Task<bool> ActivateByBrandIdAsync(string brandId);
        Task<GetAllBrandAPIViewModel> GetBrandById(string brandId);
        Task<GetBrandIdByBrandNameAPIViewModel> GetBrandIdByBrandName(string brandName);
    }
    public class BrandService : IBrandService
    {
        //fields
        private readonly IUnitOfWork _uow;
        private readonly IDapperContext _dapper;

        //constructor with params
        public BrandService(IUnitOfWork uow, IDapperContext dapper)
        {
            _uow = uow;
            _dapper = dapper;
        }

        //methods
        /// <summary>
        /// Create brand. 
        /// </summary>
        /// <param name="info"></param>
        /// <returns>Return code: 400: duplicated account; 200: success; 500: exception</returns>    
        public async Task<int> CreateBrandAsync(CreateBrandAPIViewModel info, string creatorId)
        {
            //check duplicate name brand
            Brand dbBrand = await _uow.Brand.GetFirstOrDefaultAsync(q => q.Name == info.NameBrand);
            if (dbBrand == null)
            {
                Brand newBrand = new Brand()
                {
                    Name = info.NameBrand,
                    Creator = creatorId,
                    IsActive = true,
                    Id = Guid.NewGuid().ToString(),
                };

                try
                {
                    await _uow.Brand.AddAsync(newBrand);
                    await _uow.SaveAsync();
                    return 200;
                }
                catch
                {
                    return 500;
                }
            }
            else
            {
                return 400;
            }
        }

        //Service to get all brands
        //public async Task<List<GetAllBrandAPIViewModel>> GetAllAsync()
        //{
        //    List<Brand> brands = await _uow.Brand.GetAllAsync(null, o => o.OrderByDescending(s => s.Name));
        //    if (brands != null && brands.Count > 0)
        //    {
        //        List<GetAllBrandAPIViewModel> results = new List<GetAllBrandAPIViewModel>();
        //        foreach (var brand in brands)
        //        {
        //            results.Add(new GetAllBrandAPIViewModel()
        //            {
        //                BrandId = brand.Id,
        //                BrandName = brand.Name,
        //                CreatorId = brand.Creator,
        //                IsActivate = brand.IsActive,
        //            });

        //        }
        //        return results;
        //    }
        //    else return null;
        //}

        //using dapper
        public async Task<List<GetAllBrandAPIViewModel>> GetAllAsync()
        {
            string query = @"select b.Id as BrandId ,b.Name as BrandName, a.Username as CreatorId, b.IsActive as IsActivate from Brand as b join Account as a on b.Creator = a.Id";

            using (var connection = _dapper.CreateConnection())
            {
                var result = await connection.QueryAsync<GetAllBrandAPIViewModel>(query);
                return result.ToList();
            }
        }

        //service to get an brand by brandId
        //public async Task<GetAllBrandAPIViewModel> GetBrandById(string brandId)
        //{
        //    Brand brand = await _uow.Brand.GetFirstOrDefaultAsync(q => q.Id == brandId);
        //    if (brand != null)
        //    {
        //        return new GetAllBrandAPIViewModel()
        //        {
        //            BrandName = brand.Name,
        //            BrandId = brandId
        //        };
        //    }
        //    else return null;
        //}

        //service to get a brand by brandId using Dapper
        public async Task<GetAllBrandAPIViewModel> GetBrandById(string brandId)
        {
            string query = @"select b.Id as BrandId, b.Name as BrandName, a.Username as CreatorId, b.IsActive as IsActivate from 
                            Brand as b join Account AS a on b.Creator = a.Id where b.Id = @BrandId";

            using (var connection = _dapper.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<GetAllBrandAPIViewModel>(query, new { BrandId = brandId });
                return result;
            }
        }

        //service to update a brand by BrandId
        public async Task<bool> UpdateByBrandIdAsync(UpdateBrandAPIViewModel newBrand)
        {
            Brand oldBrand = await _uow.Brand.GetFirstOrDefaultAsync(q => q.Id == newBrand.BrandId);

            if (oldBrand != null)
            {
                if (oldBrand.Name == newBrand.BrandName)
                {
                    oldBrand.Name = newBrand.BrandName;
                    try
                    {
                        _uow.Brand.Update(oldBrand);
                        await _uow.SaveAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                {
                    Brand isDuplicateName = await _uow.Brand.GetFirstOrDefaultAsync(q => q.Name == newBrand.BrandName);

                    if (isDuplicateName == null)
                    {
                        oldBrand.Name = newBrand.BrandName;
                        try
                        {
                            _uow.Brand.Update(oldBrand);
                            await _uow.SaveAsync();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else return false;
        }

        //service to delete a brand by brandId
        public async Task<bool> DeleteByBrandIdAsync(string brandId)
        {
            //check brand
            Brand brand = await _uow.Brand.GetFirstOrDefaultAsync(q => q.Id == brandId && q.IsActive);
            if (brand != null)
            {
                //change status of IsActive
                brand.IsActive = false;
                try
                {
                    //update status of account and save it
                    _uow.Brand.Update(brand);
                    await _uow.SaveAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else return false;
        }

        //service to activate a brand is deleted by brandId
        public async Task<bool> ActivateByBrandIdAsync(string brandId)
        {
            //check brand
            Brand brand = await _uow.Brand.GetFirstOrDefaultAsync(q => q.Id == brandId && !q.IsActive);
            if (brand != null)
            {
                //change status of IsActive
                brand.IsActive = !brand.IsActive;
                try
                {
                    //update status of account and save it
                    _uow.Brand.Update(brand);
                    await _uow.SaveAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else return false;
        }
        //service to get a brandId by brandName using Dapper
        public async Task<GetBrandIdByBrandNameAPIViewModel> GetBrandIdByBrandName(string brandName)
        {
            string query = @"select b.Id as BrandId from Brand as b where b.Name = @BrandName";

            using (var connection = _dapper.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<GetBrandIdByBrandNameAPIViewModel>(query, new { BrandName = brandName });
                return result;
            }
        }

    }
}
