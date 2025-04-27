using Client_API_Services.Fingerprint;
using Client_ViewModel.Fingerprint;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FingerprintController : ControllerBase
    {
        //fields
        IGenerateFingerprint _fingerprint;

        //contructor with params
        public FingerprintController(IGenerateFingerprint fingerprint)
        {
            _fingerprint = fingerprint;
        }

        //methods
        [AllowAnonymous]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(GetBrowserInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int fingerprint = await _fingerprint.GenerateFingerprintDevice(model);
                    return Ok(fingerprint);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
            else
            {
                string message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(message);
            }
        }
    }
}
