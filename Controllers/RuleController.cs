using Microsoft.AspNetCore.Mvc;
using PricingService.Models;
using PricingService.Services.Interfaces;
using System;
namespace PricingService.Controllers
{
    [ApiController]
    [Route("rules")]
    public class RuleController : ControllerBase
    {
        private readonly IPricingService _pricingService;

        public RuleController(IPricingService pricingService)
        {
            _pricingService = pricingService;
        }

        [HttpPost("create")]
        public IActionResult CreateRule([FromBody] Rule rule)
        {
            try
            {
                var createdRule = _pricingService.CreateRule(rule);
                return Created($"/rules/{createdRule.Id}", createdRule);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRule(int id, [FromBody] Rule updatedRule)
        {
            try
            {
                var rule = _pricingService.UpdateRule(id, updatedRule);
                if (rule == null)
                    return NotFound(new { error = "Rule not found" });
                return Ok(rule);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRule(int id)
        {
            try
            {
                var deleted = _pricingService.DeleteRule(id);
                if (!deleted)
                    return NotFound(new { error = "Rule not found" });
                return Ok(new { success = true, message = $"Rule {id} deleted." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
            }
        }

            [HttpGet]
            public IActionResult GetAllRules()
            {
                try
                {
                    var rules = _pricingService.GetAllRules();
                    return Ok(rules);
                }
                catch (System.Exception ex)
                {
                    return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
                }
            }

            [HttpGet("{id}")]
            public IActionResult GetRuleById(int id)
            {
                try
                {
                    var rules = _pricingService.GetAllRules();
                    var rule = rules.Find(r => r.Id == id);
                    if (rule == null)
                        return NotFound(new { error = "Rule not found" });
                    return Ok(rule);
                }
                catch (System.Exception ex)
                {
                    return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
                }
            }
    }
}
