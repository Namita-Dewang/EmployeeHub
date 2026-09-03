# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

Two independent projects that make up an employee-management app:

- `employee.api/` — ASP.NET Core 10 Web API (C#), EF Core 10 + SQL Server, controller-based.
- `frontend/` — Angular 21 SPA (standalone components, Vitest), Bootstrap 5 + FontAwesome.

There is no solution-wide build; each project is built and run on its own. The repo is not a git repository.

## Commands

### API (`cd employee.api`)
- Run (HTTP on `http://localhost:5279`): `dotnet run` or `dotnet run --launch-profile https` (adds `https://localhost:7024`)
- Build: `dotnet build`
- OpenAPI doc served at `/openapi/v1.json` in Development only (no Swagger UI).
- EF Core tools are **not** installed and there are **no migrations**. The app never calls `EnsureCreated`/`Migrate`, so the `employeeManagementDb` schema must be provisioned manually (or add migrations yourself: `dotnet tool install --global dotnet-ef` then `dotnet ef migrations add <name>` / `dotnet ef database update`).

### Frontend (`cd frontend`)
- Dev server (`http://localhost:4200`): `npm start` (= `ng serve`)
- Build: `npm run build` (production config by default; output `dist/`)
- Tests (Vitest via `@angular/build:unit-test`): `npm test` (= `ng test`)
- Single test file: `npx vitest run src/app/pages/login/login.spec.ts` (append `-t "<name>"` for one case)
- Format: `npx prettier --write .` (`.prettierrc`: single quotes, width 100)

## Architecture

### API

- **DI / startup** (`Program.cs`): registers controllers, a wide-open CORS policy `AllowAll` (any origin/method/header), and `employeeDbContext` bound to `ConnectionStrings:DefaultConnection` (SQL Server, `.\SQLEXPRESS`, in `appsettings.json`).
- **Data model** (`model/`): `employeemodel` → table `employee`, `department` → `department`, `designation` → `designation` (explicit `[Table]` attributes, lowercase names). `designation.departmentId` and `employeemodel.designationId` are plain int columns — there are **no EF navigation properties or FK relationships configured**; joins/lookups are done in the frontend.
- **Controllers** (`Controllers/`, route template `api/[controller]/[ActionName]`):
  - `EmployeeMasterController` — the mature one: `async`/`AsNoTracking`, `ModelState` checks, duplicate email/contact guards, server-set `createdDate`/`modifiedDate`. `GetEmployees` returns all; `FilterEmployees` is the real list endpoint (per-field `Contains` filters, whitelisted `sortBy`, `sortOrder`, `pageNumber`/`pageSize` 1–100, returns `{ totalRecords, pageNumber, pageSize, totalPages, data }`).
  - `Login` (on `EmployeeMasterController`) — **not real auth**: matches `email` + `contact` against the `employee` table and returns the employee record.
  - `DepartmentMasterController` — synchronous, minimal validation. `DesignationMasterController` — synchronous with try/catch + duplicate-name guards and a `FilterDesignations` endpoint.
- **Conventions**: C# identifiers are deliberately camelCase (`employeemodel`, `employeeDbContext`, property `employeeId`) — match the existing style, do not "correct" it. Errors are returned as `StatusCode(500, "<message>: " + ex.Message)` strings.
- `WeatherForecastController` / `WeatherForecast.cs` are leftover template scaffolding — ignore/remove, not part of the app.

### Frontend

- **Bootstrapping**: `main.ts` → `appConfig` (`app.config.ts`) provides router + `provideHttpClient()`. No SSR/hydration wiring is active despite a `dist/.../prerendered-routes.json` artifact.
- **Routing** (`app.routes.ts`): `''` redirects to `login`; the `Login` page is standalone; all other pages (`dashboard`, `new-employee`, `department`, `designation`, `employee-list`) render as children of the `Header` shell component (sidebar + `<router-outlet>`). **There are no route guards** — "login" only writes `localStorage['empLoginUser']`; navigating directly to an inner route is not blocked.
- **Components** live in `src/app/pages/<name>/` using Angular's newer no-suffix naming: class `EmployeeList`, files `employee-list.ts` / `.html` / `.css` / `.spec.ts`. All standalone; templates use control-flow syntax.
- **API access**:
  - `services/master.ts` (`Master`, root-provided) centralizes the shared GET calls (departments, designations, one employee, paged employees) with `retry({ count: 2, delay: 500 })` to tolerate a cold API start.
  - Writes (add/update/delete employee, login) call `HttpClient` **directly inside components** with a hardcoded `http://localhost:5279/api/...` base. The API base URL is duplicated across `master.ts`, `login.ts`, `employee-list.ts`, `employee-form.ts` — change all of them together (no environment file).
- **Employee form** (`employee-form.ts`): one component for create and edit, switched by `?id=` query param.
- **Theme** (`services/theme.service.ts`): signal-based dark/light toggle, persists to `localStorage['ph-theme']`, sets `data-theme` on `<html>`. Defaults to dark.
- **Models** (`src/app/models/`): TS interfaces mirroring the API DTOs; `createEmptyEmployee()` is the factory for new-record forms.

## Cross-cutting notes

- Frontend expects the API on port **5279** (the `http` launch profile). Run the API with plain `dotnet run` for the frontend to work without code changes.
- `empLoginUser` (logged-in employee JSON) and `ph-theme` are the only `localStorage` keys.
