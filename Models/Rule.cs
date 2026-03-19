using System;
using System.Collections.Generic;

namespace PricingService.Models
{
    public enum RuleType
    {
        // โปรโมชั่นลดราคาในช่วงเวลาที่กำหนด
        TimeWindowPromotion,
        // ค่าบริการเพิ่มเติมสำหรับพื้นที่ห่างไกล
        RemoteAreaSurcharge,
        // กำหนดราคาตามช่วงน้ำหนัก
        WeightTier
    }

    public class Rule
    {
        public int Id { get; set; } // เปลี่ยนเป็น string เพื่อความง่าย
        public RuleType Type { get; set; }
        public int Priority { get; set; }
        public DateTime EffectiveFrom { get; set; } // string เพื่อ map กับ JSON ตรง ๆ
        public DateTime EffectiveTo { get; set; }   // string เพื่อ map กับ JSON ตรง ๆ
        public bool IsActive { get; set; }
        public decimal? DiscountPercent { get; set; } // สำหรับ TimeWindowPromotion
        public List<string>? AreaList { get; set; } // สำหรับ RemoteAreaSurcharge
        public decimal? SurchargeAmount { get; set; } // สำหรับ RemoteAreaSurcharge
        public List<WeightTierItem>? WeightTiers { get; set; } // สำหรับ WeightTier
    }

    public class WeightTierItem
    {
        public double MinWeight { get; set; }
        public double MaxWeight { get; set; }
        public decimal Price { get; set; }
    }
}
