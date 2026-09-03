# Known Issues

Tracked problems that are understood but not yet fixed. Fixed items should be
removed from this file.

## 1. `npm test` (frontend) is currently red

**Status:** open — deferred, not a blocker for the current milestone.

`cd frontend && npm test` reports roughly **5 failing / 4 passing** spec files.

### Cause

The spec files generated alongside the page components were never given a
testing harness for the dependencies those components use:

- `dashboard.spec.ts`, `header.spec.ts`, `employee-form.spec.ts`,
  `employee-list.spec.ts` call
  `TestBed.configureTestingModule({ imports: [Component] })` with **no
  `provideRouter()` / `provideHttpClient()`**. Components that import
  `RouterLink` or inject `Router` throw `NullInjectorError`, and the
  `Master` service's `retry({ delay: 500 })` HTTP call outlives the test
  injector, producing `NG0205: Injector has already been destroyed`.
- `app.spec.ts` still asserts `<h1>` contains `"Hello, frontend"`, but
  `app.html` was reduced to just `<router-outlet>`, so that markup no longer
  exists.

### Fix when picked up

- Add `provideRouter([])` and `provideHttpClientTesting()` (with
  `provideHttpClient()`) to each failing spec's `providers`, and flush/verify
  HTTP with `HttpTestingController` so no request outlives the test.
- Update or delete the stale `app.spec.ts` "should render title" case.

Until then, treat frontend unit tests as non-gating and rely on `ng build` +
manual verification.
