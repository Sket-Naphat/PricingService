using System;
using System.Collections.Generic;

namespace PricingService.Models
{
    public class Job
    {
        public int JobId { get; set; } // เปลี่ยนจาก Guid เป็น int PK
        public string Status { get; set; } = "Pending";
        public List<QuoteResult> Results { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
