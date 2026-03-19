using System.Collections.Generic;

namespace PricingService.Models
{
    public class BulkQuoteRequest
    {
        public List<QuoteRequest> Requests { get; set; } = new();
    }
}
