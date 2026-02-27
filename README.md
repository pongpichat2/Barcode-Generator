# Barcode-Generator

🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Vue , TypeScript |
| Backend | .NET 10 Web API, EF Core 9, JWT Bearer |
| Database | Microsoft SQL Server 2022 (Docker) |
| Testing | xUnit, Moq, FluentAssertions |

## 🏗️ Architecture

```
HTTP Request
     │
     ▼
┌─────────────┐
│  Controller │  ← รับ/ส่ง HTTP เท่านั้น ไม่มี logic
└──────┬──────┘
       │
       ▼
┌─────────────┐
│   Service   │  ← Business Logic, Validation, Rules
└──────┬──────┘
       │
       ▼
┌─────────────┐
│ Repository  │  ← Database operations เท่านั้น
└──────┬──────┘
       │
       ▼
┌─────────────┐
│  DbContext  │  ← EF Core → MSSQL
└─────────────┘

Login

| Username | Password |
|----------|----------|
| `admin` | `Admin@1234` |
