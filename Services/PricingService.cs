using PricingService.Models;
using PricingService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace PricingService.Services
{
    public class PricingService : IPricingService
    {
        // ฟังก์ชัน generic สำหรับอ่านข้อมูลจากไฟล์ JSON
        private List<T> GetAllFromFile<T>(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<T>();
            var json = File.ReadAllText(filePath);
            var items = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return items ?? new List<T>();
        }

        // อ่าน jobs ทั้งหมดจากไฟล์ JSON
        private List<Job> GetAllJobs()
        {
            return GetAllFromFile<Job>("Data/jobs.json");
        }

        // คำนวณราคาตาม request ที่รับเข้ามา
        public QuoteResult CalculatePrice(QuoteRequest request)
        {
            try
            {
                // 1. เลือก rules ที่ตรงกับช่วงเวลาและเปิดใช้งาน
                var GetAllRules = GetAllFromFile<Rule>("Data/rules.json");
                var rules = GetAllRules
                    .Where(r => r.IsActive 
                        && r.EffectiveFrom <= request.ServiceDate 
                        && r.EffectiveTo >= request.ServiceDate)
                    .OrderBy(r => r.Priority)
                    .ToList();

                decimal price = 0;
                var appliedRules = new List<string>();
                var messages = new List<string>();

                // 2. หา base price จาก WeightTier (ถ้ามี)
                var weightRule = rules.FirstOrDefault(r => r.Type == RuleType.WeightTier && r.WeightTiers != null && r.WeightTiers.Any());
                if (weightRule?.WeightTiers != null)
                {
                    // เลือก tier ที่ตรงกับน้ำหนัก
                    var tier = weightRule.WeightTiers.FirstOrDefault(t => request.Weight >= t.MinWeight && request.Weight <= t.MaxWeight);
                    if (tier != null)
                    {
                        price = tier.Price;
                        appliedRules.Add($"WeightTier: {tier.MinWeight}-{tier.MaxWeight}kg = {tier.Price}");
                        messages.Add($"Base price: {tier.Price}");
                    }
                }

                // 3. ลดราคาตามโปรโมชั่น (TimeWindowPromotion) ถ้ามี
                var promoRule = rules.FirstOrDefault(r => r.Type == RuleType.TimeWindowPromotion && r.DiscountPercent.HasValue);
                if (promoRule != null && promoRule.DiscountPercent.HasValue)
                {
                    // คำนวณส่วนลด
                    var discount = price * promoRule.DiscountPercent.Value / 100m;
                    price -= discount;
                    appliedRules.Add($"TimeWindowPromotion: -{promoRule.DiscountPercent.Value}%");
                    messages.Add($"Promotion applied: -{discount}");
                }

                // 4. บวกค่าบริการพื้นที่ห่างไกล (RemoteAreaSurcharge) ถ้ามี
                var areaRule = rules.FirstOrDefault(r => r.Type == RuleType.RemoteAreaSurcharge && r.AreaList != null && r.SurchargeAmount.HasValue);
                if (areaRule?.AreaList != null && !string.IsNullOrEmpty(request.Destination) && areaRule.AreaList.Contains(request.Destination) && areaRule.SurchargeAmount.HasValue)
                {
                    price += areaRule.SurchargeAmount.Value;
                    appliedRules.Add($"RemoteAreaSurcharge: {request.Destination} +{areaRule.SurchargeAmount.Value}");
                    messages.Add($"Surcharge for remote area applied: {areaRule.SurchargeAmount.Value}");
                }

                // 5. คืนผลลัพธ์ราคาสุดท้าย
                return new QuoteResult
                {
                    Price = price,
                    AppliedRules = appliedRules,
                    Messages = messages
                };
            }
            catch (Exception ex)
            {
                // กรณีเกิดข้อผิดพลาด ส่ง error กลับไปที่ controller
                throw new Exception($"Error in CalculatePrice: {ex.Message}", ex);
            }
        }

        public Job SubmitBulkQuotes(BulkQuoteRequest request)
        {
            // โหลด jobs เดิมจากไฟล์

            var jobs =  GetAllFromFile<Job>("Data/jobs.json");
            // หา PK ล่าสุด แล้ว +1
            int nextId = jobs.Any() ? jobs.Max(j => j.JobId) + 1 : 1;
            // คำนวณราคาทุก request
            var results = new List<QuoteResult>();
            if (request?.Requests != null)
            {
                foreach (var req in request.Requests)
                {
                    try
                    {
                        var result = CalculatePrice(req);
                        results.Add(result);
                    }
                    catch (Exception ex)
                    {
                        // กรณี error ให้เพิ่มผลลัพธ์ error ลงใน results
                        results.Add(new QuoteResult
                        {
                            Price = 0,
                            AppliedRules = new List<string>(),
                            Messages = new List<string> { $"Error: {ex.Message}" }
                        });
                    }
                }
            }
            // สร้าง job ใหม่
            var job = new Job
            {
                JobId = nextId,
                Status = "Completed",
                Results = results,
                CreatedAt = DateTime.UtcNow
            };
            // เพิ่ม job ใหม่เข้าไป
            jobs.Add(job);
            var updatedJson = JsonSerializer.Serialize(jobs, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("Data/jobs.json", updatedJson);
            // คืน job model กลับไป
            return job;
        }

        public Job? GetJobById(int jobId)
        {
            var jobs = GetAllFromFile<Job>("Data/jobs.json");
            return jobs.FirstOrDefault(j => j.JobId == jobId);
        }
    }
}
