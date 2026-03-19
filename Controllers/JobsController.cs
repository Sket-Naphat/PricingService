using Microsoft.AspNetCore.Mvc;
using PricingService.Services.Interfaces;

namespace PricingService.Controllers
{
    [ApiController]
    [Route("jobs")]
    public class JobsController : ControllerBase
    {
        private readonly IPricingService _pricingService;

        public JobsController(IPricingService pricingService)
        {
            _pricingService = pricingService;
        }

        [HttpGet("{job_id}")]
        public IActionResult GetJobById(int job_id)
        {
            try
            {
                var job = _pricingService.GetJobById(job_id);
                if (job == null)
                    return NotFound(new { error = "Job not found" });
                return Ok(job);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
            }
        }
    }
}
