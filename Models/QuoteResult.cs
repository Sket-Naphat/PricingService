using System.Collections.Generic;

namespace PricingService.Models
{
    public class QuoteResult
    {
        public decimal Price { get; set; }
        public List<string> AppliedRules { get; set; } = new();
        public List<string> Messages { get; set; } = new();
    }
}
