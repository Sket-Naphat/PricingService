using System;

namespace PricingService.Models
{
    public class QuoteRequest
    {
        public string? Source { get; set; }
        public string? Destination { get; set; }
        public double Weight { get; set; }
        public DateTime ServiceDate { get; set; }
    }
}
