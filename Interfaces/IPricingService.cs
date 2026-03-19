using PricingService.Models;

namespace PricingService.Services.Interfaces
{
    public interface IPricingService
    {
        QuoteResult CalculatePrice(QuoteRequest request);
        Job SubmitBulkQuotes(BulkQuoteRequest request);
        Job? GetJobById(int jobId);
    }
}
