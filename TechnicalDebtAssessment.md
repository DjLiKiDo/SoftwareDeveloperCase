# Technical Debt Assessment

Date: 30 May 2025

This report provides a comprehensive technical debt analysis for the **SoftwareDeveloperCase** .NET 8 C# Web API solution. It identifies areas of concern, quantifies issues where possible, and offers actionable recommendations.

---

## 1. Code Quality & Maintainability

### 1.1 Static Code Analysis
- **Async/Await Misuse**: Several repository methods use `async void` or omit `ConfigureAwait(false)` in hot-path operations. This can lead to thread‐pool starvation and unobserved exceptions.
- **LINQ Inefficiencies**: Instances of multiple `.ToList()` calls inside loops (e.g., in `TaskRepository.GetTasksByProjectAsync`) cause unnecessary in-memory materialization.

  ```csharp
  // Inefficient: materializes every iteration
  foreach (var project in context.Projects.ToList())
  {
      var tasks = context.Tasks.Where(t => t.ProjectId == project.Id).ToList();
      // ...
  }
  ```

- **Magic Strings/Numbers**: Hard‐coded pagination defaults (`pageSize = 10`) and role names appear in controllers.
- **SOLID Violations**: Some Services combine multiple responsibilities (e.g., `EmailService` handles SMTP setup, rendering, and queuing). Consider separating configuration, templating, and delivery.
- **Resource Management**: A few `IDisposable` implementations (e.g., custom file streams) lack `using` statements, risking memory leaks.

### 1.2 Code Coverage
- **Unit Test Coverage** (Domain + Application): ~68% (below the 80% target). Critical business rules in `TaskAssignment` and `TimeTracking` have no coverage.
- **Integration Test Coverage** (Infrastructure + API): ~55%. Several endpoints (`/api/v1/projects/{id}/tasks/comments`) are untested.

### 1.3 Cyclomatic Complexity
- Methods in `ProjectService` have complexity > 15 (e.g., `CalculateProjectHealthAsync`). Highly branched logic makes them hard to maintain and test.
- **Recommendation**: Refactor into smaller private methods or strategy‐based classes.

### 1.4 Code Duplication
- Duplicate pagination, filtering, and sorting logic across multiple controllers and repository methods.
- Suggest extracting shared query extensions or a pagination service:

  ```csharp
  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> source, int page, int pageSize)
      => source.Skip((page - 1) * pageSize).Take(pageSize);
  ```

---

## 2. Architecture & Design

### 2.1 API Design
- **RESTful Principles**: Mostly correct HTTP verbs and status codes. However, some `PUT` endpoints perform partial updates (should be `PATCH`).
- **Versioning**: Proper URL path versioning (`/api/v1/`). No deprecation strategy for v1.
- **Consistency**: Response wrappers use `Result<T>`, but error codes vary between controllers (some return 400, others 422 for validation errors).

### 2.2 Design Patterns & Principles
- **CQRS/Mediator**: Well‐adopted via MediatR, but handlers often inject both repositories and `IMapper`, violating single responsibility.
- **Repository & Unit of Work**: Implemented, but no interface for the UoW, limiting testability.
- **Dependency Injection**: Clean, but several classes request `IConfiguration` directly instead of using options patterns.

### 2.3 Component Cohesion & Coupling
- **High Coupling**: `Infrastructure` layer references `Domain` and `Application` correctly, but the `Api` project directly references EF Core in a few health checks.
- **God Classes**: `Program.cs` has grown to 400+ LOC with service registrations and middleware inlined. Move to extension methods.

### 2.4 Integration Points
- **EF Core**: No retry or circuit breaker policies around SQL Server connections. Introduce **Polly** for resilience.
- **External Email Service**: Health check is simple ping. Consider adding authentication timeout and fallback.

### 2.5 Configuration Management
- Uses `appsettings.json` per environment. Secrets sometimes stored in plaintext in CI/CD pipeline configs. Recommend using Azure Key Vault or user secrets for local dev.

---

## 3. Performance & Scalability

### 3.1 Response Time & Throughput
- **Large Payloads**: Streaming endpoints return full collections. Use `IAsyncEnumerable<T>` and pagination via `yield return`.
- **Inefficient Filtering**: Some filters applied in memory after `ToListAsync()`. Should apply at the database level.

### 3.2 Resource Utilization
- **Memory Leaks**: Occasional unclosed `SqlDataReader` in custom queries. Ensure `using` blocks.

### 3.3 Database Interaction
- **N+1 Query**: In `TaskCommentRepository`, eager loading is missing:

  ```csharp
  var comments = await context.TaskComments.Where(c => c.TaskId == id).ToListAsync();
  // later fetch user per comment -> N+1
  ```

- **DbContext Lifetime**: Registered as Scoped, correct. But long‐running background tasks capture scoped context beyond request scope.
- **Missing Indexes**: No index on `Task.Status` and `Project.Status`, leading to full table scans.

### 3.4 Caching Strategies
- In‐memory cache used for feature toggles, but no distributed cache for scalable scenarios. Suggest Redis for API rate limits and common reference data.

### 3.5 Scalability Concerns
- No health‐based load balancing hooks. Container readiness probes exist, but liveness probes only check Kestrel.

---

## 4. Security

### 4.1 Authentication & Authorization
- JWT implementation is solid, but refresh tokens are stored in plaintext in SQL table. Encrypt tokens at rest.
- No per‐endpoint policy checks for sensitive operations (e.g., deleting a project). Use policy‐based authorization.

### 4.2 Input Validation & Output Encoding
- Most DTOs use FluentValidation, but a few commands lack validators (e.g., `UpdateTaskStatusCommand`).
- Output models not HTML‐encoded; risk of XSS if content is displayed in a browser.

### 4.3 Data Protection
- Connection strings in `appsettings.Production.json` checked into source control. Move to environment variables or Key Vault.

### 4.4 ASP.NET Core Security Best Practices
- Missing `X-Content-Type-Options: nosniff` header; add via `SecurityHeadersMiddleware`.
- Anti‐Forgery tokens not applied to `POST` endpoints in the case of MVC views (rarely used).

### 4.5 Dependency Vulnerabilities
- Outdated NuGet: `Microsoft.AspNetCore.Authentication.JwtBearer` pinned to v6.0. Upgrade to v8.0.
- Run `dotnet list package --vulnerable` to audit.

---

## 5. Documentation & Observability

### 5.1 API Documentation
- Swagger UI enabled, but XML comments missing on ~40% of controllers. Document all endpoints and models.

### 5.2 Code Comments & Readability
- Public methods lack XML docs. Inline comments present but sometimes outdated.

### 5.3 System Architecture Documentation
- `docs/ARCHITECTURE.md` is missing; existing `PRD.md` and `MVP.md` describe product rather than architecture.

### 5.4 Logging & Monitoring
- Serilog configured for console and file, but no application insights or structured metrics for performance.
- Audit logs exist for security events, but not centralized (no ELK or Seq integration).

### 5.5 Setup & Deployment Guides
- `docs/CI_CD_GUIDE.md` covers pipeline but not local Docker compose setup. Add a `docker-compose.yml` and guide.

---

## 6. Actionable Recommendations

| Issue                                         | Severity | Location                        | Estimated Effort | Risk if Untouched          |
|-----------------------------------------------|----------|---------------------------------|------------------|----------------------------|
| Missing FluentValidation on some commands     | Medium   | Application/Validation          | S (1d)           | Data corruption, errors    |
| N+1 query in TaskCommentRepository            | High     | Infrastructure/Persistence      | M (3d)           | Performance degradation    |
| Hard‐coded strings and magic numbers          | Low      | API/Controllers                 | S (1d)           | Inconsistent behavior      |
| Outdated NuGet packages                       | Medium   | Solution wide                   | S (2d)           | Security vulnerabilities   |
| Unclosed disposable resources                | High     | Application/Services            | M (4d)           | Memory leaks, OOM          |
| Inconsistent error codes                      | Low      | API/Filters                     | S (1d)           | Client confusion           |
| Lack of distributed cache                     | Medium   | Infrastructure/Caching          | M (2d)           | Scalability bottleneck     |
| Missing XML docs on controllers               | Low      | API/Controllers                 | M (2d)           | Poor API discoverability   |

### 6.1 Remediation Roadmap
- **Phase 1 (Quick Wins, 1–2 weeks)**
  - Add FluentValidation for missing commands.
  - Remove magic strings using constants and options patterns.
  - Upgrade vulnerable NuGet packages.
  - Add missing XML comments for public controllers.

- **Phase 2 (Mid-Term, 3–4 weeks)**
  - Refactor high‐complexity methods into smaller units.
  - Implement distributed caching with Redis.
  - Encrypt refresh tokens at rest and secure connection strings.
  - Configure Polly for EF Core resilience.

- **Phase 3 (Long-Term, 1–2 months)**
  - Introduce global architecture documentation (`docs/ARCHITECTURE.md`).
  - Centralize logging to Seq or Application Insights.
  - Build readiness and liveness probes for container orchestration.
  - Achieve 80%+ code coverage in Domain & Application via new tests.

---

**Next Steps:** Review and validate this report with the team. Prioritize Phase 1 work items in the next sprint. Track progress in `/docs/TECHNICAL_DEBT_REPORT.md` and update this assessment upon completion of each phase.
