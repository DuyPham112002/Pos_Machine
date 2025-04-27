using Client_POS_Services.AuthServices;
using Client_Services.SubCategoryServices;
using Client_ViewModel.Token;
using Microsoft.AspNetCore.Mvc;

namespace Client_POS.Controllers
{
    public class SubCategoryController : Controller
    {
        private readonly IAuthService _auth;
        private readonly ISubCategoryService _subcategory;
        public SubCategoryController(IAuthService auth, ISubCategoryService subcategory)
        {
            _auth = auth;
            _subcategory = subcategory;
        }

        [HttpGet]
        public async Task<IActionResult> GetSubCategories(string categoryId)
        {
            ISession session = HttpContext.Session;
            string token = session.GetString("AUTH");
            if (!string.IsNullOrEmpty(token))
            {
                TokenDecodedViewModel checkedToken = await _auth.CheckTokenAsync(token);
                if (checkedToken != null && checkedToken.RoleName == "Employee")
                {
                    var subCategories = await _subcategory.GetAllByCategoryAsync(categoryId, token);
                    return Json(subCategories);
                }
                return Unauthorized();
            }
            return Unauthorized();
        }
    }
}
