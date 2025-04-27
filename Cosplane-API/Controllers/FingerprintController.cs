using Client_ViewModel.Fingerprint;
using Cosplane_API_Service.DeviceService;
using Cosplane_API_Service.FingerprintService;
using Cosplane_API_ViewModel.Device;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cosplane_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FingerprintController : ControllerBase
    {
        //fields
        private readonly IDeviceService _device;
        private readonly ICheckFingerprintService _checkFingerprintService;

        //constructor with params
        public FingerprintController(IDeviceService device, ICheckFingerprintService checkFingerprintService)
        {
            _device = device;
            _checkFingerprintService = checkFingerprintService;
        }

        //methods
        [AllowAnonymous]
        [HttpPost("Check")]
        public async Task<IActionResult> CheckFingerprint(CheckFingerprintAPIViewModel model)
        {
            bool check = await _checkFingerprintService.CheckFingerprint(model.BrandId, model.DeviceFingerprint);
            if (check)
            {
                return Ok();
            }
            else return BadRequest();

        }
        [AllowAnonymous]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(GetBrowserInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int fingerprint = await _checkFingerprintService.GenerateFingerprintDevice(model);
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
