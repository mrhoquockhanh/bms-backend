# README-API-CONNECT
*(Updated & Synced with Backend Source — October 2025)*

## 1️⃣ Overview
- **Architecture:** Microservices + API Gateway (Ocelot)
- **Stack:** .NET 9, EF Core, JWT Auth, MassTransit (in-memory for dev)
- **Gateway purpose:** Central entry for all frontend requests (React/Next.js/etc)
- **Security:** JWT validation handled by API Gateway and IAM service

---

## 2️⃣ Default Ports (from launchSettings)
| Service | Port | Description |
|----------|------|-------------|
| API Gateway | 7000 (HTTPS) / 7001 (HTTP) | Entry point for frontend |
| IAM Service | 7020 (HTTPS) | Auth, user management |
| Patient Service | 7263 (HTTPS) | Patient info & medical records |
| Test Order Service | 7105 (HTTPS) | Test orders, doctors, appointments |
| Payment Service | 49700 (HTTPS) | VNPay flows, payment queries |
| Warehouse Service | 53266 (HTTPS) | Equipment & reagents |

---

## 3️⃣ How to Run (Dev Mode)

### Option A — Run individually

```sh
dotnet run --project api-gateway
dotnet run --project iam-service
dotnet run --project Patient-service
dotnet run --project Testorder-service
dotnet run --project Payment-service
dotnet run --project warehouse-service

```

### Option B — Docker Compose (if available)

```sh
docker compose up --build

```

---

## 4️⃣ Authentication (JWT)
- **Login flow:**
  1. `POST /gateway/iam/auth/login` → returns JWT token  
  2. Include in requests as:

```sh
     Authorization: Bearer <token>

```
- **Token issuer:** IAM service  
- **Validated by:** API Gateway  
- **Direct service access:** Patient & TestOrder currently do **not validate JWT internally** (trust Gateway).  
  → To enable defense-in-depth, add `UseAuthentication()` and `UseAuthorization()` in each service’s `Program.cs`.

---

## 5️⃣ Configuration (JWT & Security)
| Setting | IAM | API Gateway | Notes |
|----------|-----|--------------|-------|
| Key | Jwt:Key | JwtSettings:Secret or JWT_SECRET (env) | Should match |
| Issuer | Jwt:Issuer | JwtSettings:Issuer | Must be same |
| Audience | Jwt:Audience | JwtSettings:Audience | Must be same |

**Recommendation:** Unify the config section name to `Jwt` for both IAM and Gateway.

---

## 6️⃣ Ocelot Configuration
- Config file: `/api-gateway/ocelot.json`
- `DownstreamScheme` is `https` for all services → ensure dev certs are trusted (`dotnet dev-certs https --trust`).
- Routes requiring authentication include:

```json
  "AuthenticationOptions": { "AuthenticationProviderKey": "Bearer" }

```
- All `/gateway/iam/*` and `/gateway/admin/*` routes require a valid JWT.

---

## 7️⃣ CORS
Configured in API Gateway:

```csharp
http://localhost:3000
https://localhost:3000

```
➡️ Update `Program.cs` if the frontend runs on another domain.

---

## 8️⃣ Frontend integration notes
- Always call APIs through the Gateway base `https://localhost:7000` using the `/gateway/...` paths.
- Do not call downstream service ports from the browser.
- Send `Authorization: Bearer <token>` for protected admin routes.
- Handle 401/403 globally in the frontend; re-login on 401.

## 8️⃣ Data Protection (Cross-Platform Note)
Currently stores DataProtection keys at:

```
/root/.aspnet/DataProtection-Keys

```
✅ Works in Docker/Linux  
⚠️ On Windows, replace with:

```csharp
Path.Combine(
  Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
  "DataProtection-Keys"
)

```
Or use environment variables for cross-platform support.

---

## 9️⃣ Main API Endpoints

### 🧑‍⚕️ IAM (Authentication)
| Method | Endpoint | Auth | Description |
|---------|-----------|------|-------------|
| POST | /gateway/iam/auth/login | ❌ | Login & receive JWT |
| POST | /gateway/iam/auth/register | ❌ | Register by email |
| POST | /gateway/iam/auth/otp/send | ❌ | Send OTP |
| POST | /gateway/iam/auth/otp/verify | ❌ | Verify OTP |

### 👤 Admin (IAM)
| Method | Endpoint | Auth | Description |
|---------|-----------|------|-------------|
| GET/POST | /gateway/admin/accounts | ✅ | List or create accounts |
| GET/PUT/DELETE | /gateway/admin/accounts/{id} | ✅ | Account by id |

---

### 🩺 Patient Service
| Method | Endpoint | Auth | Description |
|---------|-----------|------|-------------|
| GET/POST | /gateway/patients | ❌ | List or create patients |
| GET | /gateway/patients/by-id/{id} | ❌ | Get patient by ID |
| GET | /gateway/patients/by-email/{email} | ❌ | Get patient by email |
| GET | /gateway/patients/search?key=term | ❌ | Search patients |
| POST | /gateway/patients/register | ❌ | Register patient + IAM account |
| PUT/DELETE | /gateway/patients/{id} | ❌ | Update/Delete patient |

**Submodules**
- Medical Records:
  - GET/POST `/gateway/patients/medical-records`
  - GET `/gateway/patients/medical-records/{id}`
  - GET `/gateway/patients/medical-records/by-patient/{patientId}`
  - GET `/gateway/patients/medical-records/search?key=...`
  - PUT/DELETE `/gateway/patients/medical-records/{id}`
- Lab Tests:
  - GET/POST `/gateway/patients/lab-tests`
  - GET `/gateway/patients/lab-tests/{id}`
  - GET `/gateway/patients/lab-tests/search?key=...`
- Lab Criteria:
  - GET/POST `/gateway/patients/lab-criteria`
  - GET `/gateway/patients/lab-criteria/{id}`
  - GET `/gateway/patients/lab-criteria/search?key=...`
  - PUT/DELETE `/gateway/patients/lab-criteria/{id}`

---

### 🧪 Test Order Service
| Method | Endpoint | Auth | Description |
|---------|-----------|------|-------------|
| GET/POST | /gateway/testorder/appointments | ❌ | Appointments |
| GET/PUT/DELETE | /gateway/testorder/appointments/{id} | ❌ | Appointment by id |
| GET | /gateway/testorder/appointments/patient/{patientId} | ❌ | Appointments of patient |
| GET/POST | /gateway/testorder/doctor | ❌ | Doctors |
| GET/PUT/DELETE | /gateway/testorder/doctor/{id} | ❌ | Doctor by id |
| GET | /gateway/testorder/doctor/email/{email} | ❌ | Doctor by email |
| GET/POST | /gateway/testorder/doctorshift | ❌ | Doctor shifts |
| GET/PUT/DELETE | /gateway/testorder/doctorshift/{id} | ❌ | Doctor shift by id |
| GET/POST | /gateway/testorder/bloodsample | ❌ | Blood samples |
| GET/PUT/DELETE | /gateway/testorder/bloodsample/{id} | ❌ | Blood sample by id |
| GET/POST | /gateway/testorder/testcriterion | ❌ | Test criteria |
| GET/PUT/DELETE | /gateway/testorder/testcriterion/{id} | ❌ | Test criterion by id |
| GET/POST | /gateway/testorder/testordercomment | ❌ | Test order comments |
| GET/PUT/DELETE | /gateway/testorder/testordercomment/{id} | ❌ | Comment by id |
| GET | /gateway/testorder/testordercomment/appointment/{appointmentId} | ❌ | Comments of appointment |
| GET/POST | /gateway/testorder/testresult | ❌ | Test results |
| GET/PUT/DELETE | /gateway/testorder/testresult/{id} | ❌ | Test result by id |
| GET/POST | /gateway/testorder/testparameter | ❌ | Test parameters |
| GET/PUT/DELETE | /gateway/testorder/testparameter/{id} | ❌ | Test parameter by id |
| GET/POST | /gateway/testorder/flaggingconfiguration | ❌ | Flagging configuration |
| GET/PUT/DELETE | /gateway/testorder/flaggingconfiguration/{id} | ❌ | Flagging configuration by id |
| GET/POST | /gateway/testorder/flaggingconfiglog | ❌ | Flagging config logs |
| GET/PUT/DELETE | /gateway/testorder/flaggingconfiglog/{id} | ❌ | Flagging config log by id |
| GET/POST | /gateway/testorder/testpackage | ❌ | Test packages |
| GET/PUT/DELETE | /gateway/testorder/testpackage/{id} | ❌ | Test package by id |
| GET/POST | /gateway/testorder/testpackagecriterion | ❌ | Test package criteria |
| GET/PUT/DELETE | /gateway/testorder/testpackagecriterion/{id} | ❌ | Test package criterion by id |

Note: TestOrder does not validate JWT internally; gateway routes are open by default.

### 💳 Payment Service
| Method | Endpoint | Auth | Description |
|---------|-----------|------|-------------|
| GET | /gateway/payment/vnpay/create-payment-url | ❌ | Create VNPay URL |
| GET | /gateway/payment/vnpay/ipn | ❌ | VNPay IPN callback |
| GET | /gateway/payment/vnpay/callback | ❌ | Frontend return URL |
| GET | /gateway/payment/payments | ❌ | List payments |
| GET | /gateway/payment/payments/{id} | ❌ | Payment by id |
| GET | /gateway/payment/payments/by-patient/{patientId} | ❌ | Payments of patient |

### 🏭 Warehouse Service
| Method | Endpoint | Auth | Description |
|---------|-----------|------|-------------|
| GET/POST | /gateway/warehouse/equipment | ❌ | Equipment |
| GET/PUT/DELETE | /gateway/warehouse/equipment/{id} | ❌ | Equipment by id |
| GET/POST | /gateway/warehouse/reagents | ❌ | Reagents |
| GET/PUT/DELETE | /gateway/warehouse/reagents/{id} | ❌ | Reagent by id |

---

## 🔐 10️⃣ Security Notes
- **JWT validated by:** IAM & API Gateway  
- **JWT not validated:** Patient, TestOrder, Payment, Warehouse (trust Gateway)  
- **Defense-in-depth:** enable JWT validation inside services for direct access protection  
- **Production:** enforce HTTPS only

---

## 🧭 11️⃣ Smoke Tests

### Trust dev HTTPS cert

```sh
dotnet dev-certs https --trust

```

### 1️⃣ Login & get token

```sh
curl -X POST https://localhost:7000/gateway/iam/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"YourPassword"}'

```

### 2️⃣ Use token for protected routes

```sh
curl https://localhost:7000/gateway/admin/accounts \
  -H "Authorization: Bearer <token>"

```

### 3️⃣ Test open routes

```sh
# Patient by email
curl "https://localhost:7000/gateway/patients/by-email/test@example.com"

# TestOrder - list doctors
curl https://localhost:7000/gateway/testorder/doctor

# Warehouse - list equipment
curl https://localhost:7000/gateway/warehouse/equipment

```

---

## 🔎 Full API Endpoint Inventory (internal and via Gateway)
This section enumerates endpoints discovered in the workspace and how they map through Ocelot. Use the internal `/...` paths for direct-service testing and `/gateway/...` for frontend usage through the API Gateway.

### A. IAM Service (iam-service)
Internal base: `https://localhost:7020/IAM`
Gateway base: `https://localhost:7000/gateway/iam`

- Authentication
  - Internal: `POST /IAM/auth/login` → Gateway: `POST /gateway/iam/auth/login` (none)
  - Internal: `POST /IAM/auth/register` → Gateway: `POST /gateway/iam/auth/register` (none)
  - Internal: `POST /IAM/auth/otp/send` → Gateway: `POST /gateway/iam/auth/otp/send` (none)
  - Internal: `POST /IAM/auth/otp/verify` → Gateway: `POST /gateway/iam/auth/otp/verify` (none)

- Admin / Account management (protected via Ocelot)
  - Internal: `GET/POST /IAM/account` → Gateway: `GET/POST /gateway/admin/accounts` (JWT)
  - Internal: `GET/PUT/DELETE /IAM/account/{id}` → Gateway: `GET/PUT/DELETE /gateway/admin/accounts/{id}` (JWT)

### B. Patient Service (Patient-service)
Internal base: `https://localhost:7263/Patients`
Gateway base: `https://localhost:7000/gateway/patients`

- Patients
  - Internal: `GET /patients` → Gateway: `GET /gateway/patient` (list)
  - Internal: `GET /patients/{id}` → Gateway: `GET /gateway/patient/{id}`
  - Internal: `GET /patients/email/{email}` *(recommended internal route)* → Gateway: `GET /gateway/patient/email/{email}`
    - Note: controller code currently has conflicting routes; recommended change above
  - Internal: `GET /patients/search?key=...` → Gateway: `GET /gateway/patient/search?key=...`
  - Internal: `POST /patients` → Gateway: `POST /gateway/patient`
  - Internal: `POST /patients/register` → Gateway: `POST /gateway/patient/register` (also triggers IAM gRPC quick register)
  - Internal: `PUT /patients/{id}` → Gateway: `PUT /gateway/patient/{id}`
  - Internal: `DELETE /patients/{id}` → Gateway: `DELETE /gateway/patient/{id}`

- Medical Records (controller: `patients/medical-records`)
  - Internal: `GET /patients/medical-records` → Gateway: `GET /gateway/patient/medical-records`
  - Internal: `GET /patients/medical-records/{id}` → Gateway: `GET /gateway/patient/medical-records/{id}`
  - Internal: `GET /patients/medical-records/by-patient/{patientId}` → Gateway: `GET /gateway/patient/medical-records/by-patient/{patientId}`
  - Internal: `GET /patients/medical-records/search?key=...` → Gateway: `GET /gateway/patient/medical-records/search?key=...`
  - Internal: `POST /patients/medical-records` → Gateway: `POST /gateway/patient/medical-records`
  - Internal: `PUT /patients/medical-records/{id}` → Gateway: `PUT /gateway/patient/medical-records/{id}`
  - Internal: `DELETE /patients/medical-records/{id}` → Gateway: `DELETE /gateway/patient/medical-records/{id}`

- Lab Tests (controller: `patients/lab-tests`)
  - Internal: `GET /patients/lab-tests` → Gateway: `GET /gateway/patient/lab-tests`
  - Internal: `GET /patients/lab-tests/{id}` → Gateway: `GET /gateway/patient/lab-tests/{id}`
  - Internal: `GET /patients/lab-tests/search?key=...` → Gateway: `GET /gateway/patient/lab-tests/search?key=...`
  - Internal: `POST /patients/lab-tests` → Gateway: `POST /gateway/patient/lab-tests`
  - Internal: `PUT /patients/lab-tests/{id}` → Gateway: `PUT /gateway/patient/lab-tests/{id}`
  - Internal: `DELETE /patients/lab-tests/{id}` → Gateway: `DELETE /gateway/patient/lab-tests/{id}`

### C. Test Order Service (Testorder-service)
Internal base: `https://localhost:7105/TestOrder`
Gateway base: `https://localhost:7000/gateway/testorder`

Routes proxied by Ocelot match the `TestOrder/[controller]` attribute routes and include all modules listed in the section above. No routes are marked protected.

### D. Payment Service
Internal base: `https://localhost:49700/Payments`
Gateway base: `https://localhost:7000/gateway/payment`

- VNPay
  - Internal: `GET /Vnpay/CreatePaymentUrl` → Gateway: `GET /gateway/payment/vnpay/create-payment-url`
  - Internal: `GET /Vnpay/IpnAction` → Gateway: `GET /gateway/payment/vnpay/ipn`
  - Internal: `GET /Vnpay/Callback` → Gateway: `GET /gateway/payment/vnpay/callback`
- Payments
  - Internal: `GET /Payments` → Gateway: `GET /gateway/payment/payments`
  - Internal: `GET /Payments/{id}` → Gateway: `GET /gateway/payment/payments/{id}`
  - Internal: `GET /Payments/by-patient/{patientId}` → Gateway: `GET /gateway/payment/payments/by-patient/{patientId}`

### E. Warehouse Service
Internal base: `https://localhost:53266/Warehouse`
Gateway base: `https://localhost:7000/gateway/warehouse`

- Equipment
  - Internal: `GET/POST /Warehouse/Equipment` → Gateway: `GET/POST /gateway/warehouse/equipment`
  - Internal: `GET/PUT/DELETE /Warehouse/Equipment/{id}` → Gateway: `GET/PUT/DELETE /gateway/warehouse/equipment/{id}`
- Reagents
  - Internal: `GET/POST /Warehouse/Reagents` → Gateway: `GET/POST /gateway/warehouse/reagents`
  - Internal: `GET/PUT/DELETE /Warehouse/Reagents/{id}` → Gateway: `GET/PUT/DELETE /gateway/warehouse/reagents/{id}`

---

## ⚙️ 12️⃣ Known Issues (Oct 2025)
| Issue | Impact | Fix |
|--------|---------|-----|
| `PatientsController` `[HttpGet("{id}")]` vs `[HttpGet("{email}")]` | Routing conflict | Mapped via `/by-id/{id}` and `/by-email/{email}` in Gateway |
| DataProtection path hardcoded for Linux | Fails on Windows | Use cross-platform path |
| JWT section mismatch | Confusion | Use unified `Jwt` config |
| Testorder auth middleware inconsistency | Misleading | Remove or complete JWT config |
| HTTPS in Ocelot | Requires dev cert trust | Use `dotnet dev-certs https --trust` |

---

## ✅ 13️⃣ Summary
| Category | Status | Notes |
|-----------|---------|-------|
| JWT (Gateway & IAM) | ✅ | Consistent |
| Routes | ✅ | Synced with controllers |
| HTTPS Config | ✅ | Dev certs trusted |
| CORS | ✅ | Localhost ready |
| DataProtection | ⚠️ | Needs cross-platform |
| TestOrder/Auth | ⚠️ | Gateway-only auth for now |

---

### Changes made
- Synced ports from all `launchSettings.json`.
- Corrected IAM downstream paths (`/IAM/...`).
- Added full Patient submodules (medical-records, lab-tests, lab-criteria).
- Synced TestOrder routes to `TestOrder/[controller]` and added all modules.
- Added Payment and Warehouse services to gateway.
- Added curl examples and run/test guidance.

© 2025 — API Gateway Integration Documentation (Microservice Stack)
