using PricingService.Models;

namespace PricingService.Services.Interfaces
{
    public interface IPricingService
    {
        QuoteResult CalculatePrice(QuoteRequest request);
        Job SubmitBulkQuotes(BulkQuoteRequest request);
        Job? GetJobById(int jobId);
        Rule CreateRule(Rule rule);
        void SaveAllRules(List<Rule> rules);
        List<Rule> GetAllRules();
        Rule? UpdateRule(int id, Rule updatedRule);
        bool DeleteRule(int id);
    }
}
