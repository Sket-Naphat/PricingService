# PricingService

**TH: ระบบคำนวณราคาขนส่ง (Rule-based) ด้วย .NET 8 Core Web API**

โปรเจคนี้เป็นตัวอย่าง REST API สำหรับคำนวณราคาขนส่ง รองรับโปรโมชั่น, ค่าพื้นที่ห่างไกล, ราคาตามน้ำหนัก และออกแบบให้ขยายต่อได้ง่าย เหมาะกับงานทดสอบ/เรียนรู้ .NET 8

---

## วิธีเริ่มต้น (ภาษาไทย)

1. ติดตั้ง .NET 8 SDK
2. เปิดเทอร์มินัลในโฟลเดอร์นี้ แล้วรัน:
   ```powershell
   dotnet run
   ```
3. API จะรันที่ `http://localhost:5228`
4. เปิดดูเอกสาร API ได้ที่ [http://localhost:5228/swagger](http://localhost:5228/swagger)

---

## Architecture Overview (EN)
- **Controllers/**: API endpoints for Quotes, Jobs, Rules, Health
- **Services/**: Business logic (PricingService) and interfaces
- **Models/**: Data models for requests, results, rules, jobs, etc.
- **Data/**: JSON files for rules and jobs (sample data)
- **Swagger**: Built-in API documentation at `/swagger`

The system uses Dependency Injection (DI) and follows SOLID principles for maintainability.

---

## ตัวอย่างการเรียกใช้งาน (Sample Requests)

### คำนวณราคาทันที (POST /quotes/price)
```json
{
  "serviceDate": "2026-03-20T00:00:00",
  "weight": 10,
  "source": "Bangkok",
  "destination": "ChiangMai"
}
```

### คำนวณราคาหลายรายการ (POST /quotes/bulk)
```json
{
  "requests": [
    {
      "serviceDate": "2026-03-20T00:00:00",
      "weight": 5,
      "source": "Bangkok",
      "destination": "Bangkok"
    },
    {
      "serviceDate": "2026-03-20T00:00:00",
      "weight": 20,
      "source": "Bangkok",
      "destination": "ChiangMai"
    }
  ]
}
```

### ดูสถานะงาน (GET /jobs/{job_id})
```
GET http://localhost:5228/jobs/1
```

### Health Check (GET /health)
```
GET http://localhost:5228/health
```

---

## ตัวอย่าง CRUD ของ RULE

### สร้าง Rule (POST /rules/create)

#### 1. TimeWindowPromotion (type: 0)
```json
{
  "type": 0,
  "priority": 1,
  "effectiveFrom": "2026-03-20T00:00:00",
  "effectiveTo": "2026-04-01T23:59:59",
  "isActive": true,
  "discountPercent": 10
}
```
```json
{
  "type": 0,
  "priority": 2,
  "effectiveFrom": "2026-04-02T00:00:00",
  "effectiveTo": "2026-04-10T23:59:59",
  "isActive": true,
  "discountPercent": 5
}
```

#### 2. RemoteAreaSurcharge (type: 1)
```json
{
  "type": 1,
  "priority": 1,
  "effectiveFrom": "2026-03-20T00:00:00",
  "effectiveTo": "2026-12-31T23:59:59",
  "isActive": true,
  "areaList": ["ChiangMai", "MaeHongSon"],
  "surchargeAmount": 50
}
```
```json
{
  "type": 1,
  "priority": 2,
  "effectiveFrom": "2026-03-20T00:00:00",
  "effectiveTo": "2026-12-31T23:59:59",
  "isActive": true,
  "areaList": ["Phuket", "Satun"],
  "surchargeAmount": 70
}
```

#### 3. WeightTier (type: 2)
```json
{
  "type": 2,
  "priority": 1,
  "effectiveFrom": "2026-03-20T00:00:00",
  "effectiveTo": "2026-12-31T23:59:59",
  "isActive": true,
  "weightTiers": [
    { "minWeight": 0, "maxWeight": 10, "price": 100 },
    { "minWeight": 10.01, "maxWeight": 20, "price": 180 }
  ]
}
```
```json
{
  "type": 2,
  "priority": 2,
  "effectiveFrom": "2026-03-20T00:00:00",
  "effectiveTo": "2026-12-31T23:59:59",
  "isActive": true,
  "weightTiers": [
    { "minWeight": 20.01, "maxWeight": 30, "price": 250 },
    { "minWeight": 30.01, "maxWeight": 50, "price": 400 }
  ]
}
```

### แก้ไข Rule (PUT /rules/{id})

#### TimeWindowPromotion (type: 0)
```json
{
  "type": 0,
  "priority": 3,
  "effectiveFrom": "2026-04-05T00:00:00",
  "effectiveTo": "2026-04-15T23:59:59",
  "isActive": false,
  "discountPercent": 15
}
```

#### RemoteAreaSurcharge (type: 1)
```json
{
  "type": 1,
  "priority": 2,
  "effectiveFrom": "2026-03-25T00:00:00",
  "effectiveTo": "2026-12-31T23:59:59",
  "isActive": true,
  "areaList": ["Nan", "Tak"],
  "surchargeAmount": 80
}
```

#### WeightTier (type: 2)
```json
{
  "type": 2,
  "priority": 1,
  "effectiveFrom": "2026-03-20T00:00:00",
  "effectiveTo": "2026-12-31T23:59:59",
  "isActive": true,
  "weightTiers": [
    { "minWeight": 0, "maxWeight": 5, "price": 60 },
    { "minWeight": 5.01, "maxWeight": 15, "price": 120 }
  ]
}
```

### ลบ Rule (DELETE /rules/{id})
```
DELETE http://localhost:5228/rules/1
```

### ดู Rule ทั้งหมด (GET /rules)
```
GET http://localhost:5228/rules
```

### ดู Rule ตาม id (GET /rules/{id})
```
GET http://localhost:5228/rules/1
```

---

## ข้อมูลตัวอย่าง (Sample Data)
- `Data/rules.json`: กติกาการคิดราคา (rules)
- `Data/jobs.json`: ตัวอย่างงานที่เคยคำนวณ

## หมายเหตุ
- สามารถปรับแต่ง/ขยาย logic ได้ตามต้องการ
- มี Swagger UI สำหรับทดลอง API

---

## รันและจัดการ PricingService ด้วย Docker/Docker Compose

### 🚀 วิธีรันด้วย Docker Compose (แนะนำสำหรับมือใหม่)
1. เปิด PowerShell ในโฟลเดอร์โปรเจ็คนี้
2. สั่ง build และรัน container พร้อมกัน:
   ```powershell
   docker-compose up --build
   ```
3. รอจนเห็นข้อความ `Now listening on: http://0.0.0.0:5228` หรือ `Application started...`
4. เปิดเบราว์เซอร์ไปที่ [http://localhost:5228/swagger](http://localhost:5228/swagger) เพื่อทดสอบ API

**หยุดและลบ container:**
```powershell
docker-compose down
```
---

### 💡 หมายเหตุ
- ทุกครั้งที่แก้ไขโค้ด ต้อง build ใหม่ (`docker-compose up --build` หรือ `docker build ...`)
- ถ้าเข้า Swagger ไม่ได้ ให้ตรวจสอบว่า container รันอยู่ และลอง refresh
- ถ้าเจอปัญหา Docker ให้ลอง restart Docker Desktop

---

## Copilot Instructions
See `.github/copilot-instructions.md` for workspace-specific Copilot guidance.
