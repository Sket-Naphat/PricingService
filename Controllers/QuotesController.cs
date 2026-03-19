using Microsoft.AspNetCore.Mvc;
using PricingService.Models;
using PricingService.Services.Interfaces;
using System;

namespace PricingService.Controllers
{
    [ApiController]
    [Route("quotes")]
    public class QuotesController : ControllerBase
    {
        private readonly IPricingService _pricingService;

        public QuotesController(IPricingService pricingService)
        {
            _pricingService = pricingService;
        }

        [HttpPost("price")]
        public ActionResult<QuoteResult> CalculatePrice([FromBody] QuoteRequest request)
        {
            try
            {
                var result = _pricingService.CalculatePrice(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // สามารถ log ex ได้ที่นี่
                return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
            }
        }

        [HttpPost("bulk")]
        public ActionResult<object> SubmitBulkQuotes([FromBody] BulkQuoteRequest request)
        {
            try
            {
                // เรียกใช้ PricingService เพื่อสร้าง job และคืน job model
                var job = _pricingService.SubmitBulkQuotes(request);
                return Ok(new { job_id = job.JobId, job });
            }
            catch (Exception ex)
            {
                // log ex ได้ที่นี่
                return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
            }
        }
    }
}
