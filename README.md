# Employee Management App

A small employee-management application split into two independent projects:

| Project | Stack | Location |
| --- | --- | --- |
| **API** | ASP.NET Core 10 Web API (C#), EF Core 10, SQL Server | `employee.api/` |
| **Frontend** | Angular 21 SPA (standalone components), Bootstrap 5, FontAwesome | `frontend/` |

There is no solution-wide build — each project is built and run on its own.

## Features

- Employee CRUD with server-side filtering, sorting and pagination
- Department and designation master data management
- Dashboard summary view
- Basic login (email + contact match against the `employee` table — **not real authentication**)
- Signal-based dark/light theme toggle (persisted to `localStorage`)

## Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) with npm 11+
- SQL Server / SQL Server Express (`.\SQLEXPRESS`), database `employeeManagementDb`

## Getting started

### 1. Database

The API does **not** run migrations or `EnsureCreated` — the `employeeManagementDb`
schema must be provisioned manually. Adjust the connection string in
`employee.api/appsettings.json` if your SQL Server instance differs.

To add EF Core migrations yourself:

```bash
dotnet tool install --global dotnet-ef
cd employee.api
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 2. API

```bash
cd employee.api
dotnet run                       # HTTP on http://localhost:5279
dotnet run --launch-profile https  # also HTTPS on https://localhost:7024
```

- OpenAPI document: `http://localhost:5279/openapi/v1.json` (Development only, no Swagger UI)
- CORS policy `AllowAll` is wide open (any origin/method/header)

> The frontend expects the API on port **5279**. Run the API with plain
> `dotnet run` for the frontend to work without code changes.

### 3. Frontend

```bash
cd frontend
npm install
npm start          # dev server on http://localhost:4200
```

Other frontend commands:

```bash
npm run build      # production build -> dist/
npm test           # Vitest via @angular/build:unit-test
npx prettier --write .
```

## API endpoints

Route template: `api/[controller]/[ActionName]`

### EmployeeMasterController

| Method | Route | Notes |
| --- | --- | --- |
| GET | `api/EmployeeMaster/GetEmployees` | all employees |
| GET | `api/EmployeeMaster/GetEmployee/{id}` | single employee |
| POST | `api/EmployeeMaster/AddEmployee` | duplicate email/contact guard |
| PUT | `api/EmployeeMaster/UpdateEmployee` | |
| DELETE | `api/EmployeeMaster/DeleteEmployee/{id}` | |
| GET | `api/EmployeeMaster/FilterEmployees` | per-field filters, whitelisted sort, paging (returns `{ totalRecords, pageNumber, pageSize, totalPages, data }`) |
| POST | `api/EmployeeMaster/Login` | matches email + contact, returns employee record |

### DepartmentMasterController

`GetAllDepartments`, `AddDepartment`, `UpdateDepartment`, `DeleteDepartment/{id}`

### DesignationMasterController

`GetDesignations`, `GetDesignation/{id}`, `AddDesignation`, `UpdateDesignation`,
`DeleteDesignation/{id}`, `FilterDesignations`

## Frontend routes

| Path | Component | Notes |
| --- | --- | --- |
| `/login` | `Login` | standalone, writes `localStorage['empLoginUser']` |
| `/dashboard` | `Dashboard` | rendered inside the `Header` shell |
| `/employee-list` | `EmployeeList` | |
| `/new-employee` | `EmployeeForm` | create/edit switched by `?id=` query param |
| `/department` | `Department` | |
| `/designation` | `Designation` | |

There are **no route guards** — navigating directly to an inner route is not blocked.

## Notes

- The data model has no EF navigation properties or FK relationships; lookups/joins
  are done in the frontend.
- The API base URL (`http://localhost:5279/api/...`) is hardcoded and duplicated
  across `services/master.ts`, `login.ts`, `employee-list.ts` and
  `employee-form.ts` — change them together (there is no environment file).
- `WeatherForecastController` / `WeatherForecast.cs` are leftover template
  scaffolding and are not part of the app.
