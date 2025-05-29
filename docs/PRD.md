## 1. Product Requirements Document (PRD)

### Project Overview and Objectives

**Project Name:** SoftwareDeveloperCase - Enterprise User Management System

**Overview:** A RESTful API solution implementing a hierarchical role-based access control (RBAC) system with user management, department organization, and automated notifications.

**Objectives:**
- Provide secure user authentication and authorization
- Implement hierarchical role management with permission inheritance
- Enable department-based user organization
- Automate manager notifications for new user registrations
- Ensure data integrity and prevent circular role dependencies

### Target Audience

**Primary Users:**
- System Administrators: Managing users, roles, and permissions
- Department Managers: Overseeing department users and receiving notifications
- Employees: Basic system access with read permissions
- IT Teams: Integrating with the API for enterprise applications

**Secondary Users:**
- HR Departments: User onboarding and role assignments
- Security Teams: Monitoring access control and permissions
- DevOps Teams: System deployment and maintenance

### Core Functionality

1. **User Management**
   - User registration with email validation
   - User profile updates
   - User deletion
   - Password management
   - Department assignment

2. **Role Management**
   - Hierarchical role creation
   - Role inheritance (Employee → Manager)
   - Prevention of recursive role definitions
   - Dynamic role assignment to users

3. **Permission System**
   - CRUD permissions (Read, Add, Update, Delete)
   - Role-based permission assignment
   - Permission inheritance through role hierarchy
   - User permission calculation based on assigned roles

4. **Department Organization**
   - Department-based user grouping
   - Manager identification within departments
   - Automated notifications to department managers

5. **Security Features**
   - Email uniqueness validation
   - Input validation using FluentValidation
   - Audit trail (CreatedBy, ModifiedBy timestamps)
   - Secure password storage

### Technical Requirements

- **.NET 8 / C# 13** runtime environment
- **SQL Server** database (or compatible)
- **SMTP Server** for email notifications
- **HTTPS** enabled web server
- **JWT** token infrastructure (planned)
- **API Documentation** via Swagger/OpenAPI
- **Structured Logging** with Serilog

### Key Performance Indicators (KPIs)

1. **System Performance**
   - API response time < 200ms for GET operations
   - API response time < 300ms for POST/PUT operations
   - 99.9% uptime availability

2. **User Adoption**
   - User registration success rate > 95%
   - Role assignment completion rate > 98%
   - Permission query accuracy = 100%

3. **Security Metrics**
   - Zero security breaches
   - 100% input validation coverage
   - Complete audit trail for all operations

### Success Metrics

1. **Functional Success**
   - All users can be assigned appropriate roles
   - Permissions correctly inherited through role hierarchy
   - Managers receive notifications within 1 minute of user registration
   - No circular role dependencies created

2. **Technical Success**
   - 80%+ unit test coverage
   - Zero critical security vulnerabilities
   - Clean Architecture principles maintained
   - API documentation 100% complete

3. **Operational Success**
   - Deployment time < 30 minutes
   - System recoverable within 15 minutes
   - Database backup/restore functional
   - Monitoring and alerting operational

## 2. Minimum Viable Product (MVP) Definition

### Essential Features for MVP

1. **User Management Core**
   - User registration with email validation
   - User deletion
   - Basic user profile (name, email, department)

2. **Role System Foundation**
   - Pre-defined Employee and Manager roles
   - Role assignment to users
   - Basic role hierarchy (Manager inherits from Employee)

3. **Permission Framework**
   - Four basic permissions (Read, Add, Update, Delete)
   - Default permission sets (Employee: Read only, Manager: All)
   - User permission query endpoint

4. **Department Integration**
   - User-department association
   - Manager identification logic

5. **Basic Security**
   - Email uniqueness validation
   - Input validation on all endpoints
   - Audit fields (CreatedBy, CreatedOn, etc.)

### Scope Boundaries

**In Scope:**
- RESTful API endpoints for all core operations
- Database persistence with EF Core
- Basic email notifications (manager alerts)
- Swagger documentation
- Unit tests for business logic

**Out of Scope for MVP:**
- User authentication (JWT implementation)
- Password reset functionality
- User profile pictures
- Advanced role templates
- Permission groups/categories
- Multi-tenancy support
- Frontend application
- Advanced reporting

### Technical Constraints

1. **Database**: SQL Server only (no multi-database support)
2. **Email**: Basic SMTP integration (no queuing)
3. **Caching**: Not implemented in MVP
4. **Performance**: Synchronous operations only
5. **Scalability**: Single server deployment
6. **API Versioning**: v1 only

### Acceptance Criteria

1. **User Registration**
   - Can create user with unique email
   - Automatically assigns Employee role
   - Sends notification to department manager
   - Returns user ID on success

2. **Role Management**
   - Can create roles with parent hierarchy
   - Prevents circular dependencies
   - Can assign/remove roles from users

3. **Permission System**
   - Can assign permissions to roles
   - Can query user's effective permissions
   - Correctly inherits permissions through role hierarchy

4. **API Quality**
   - All endpoints return consistent response format
   - Proper HTTP status codes used
   - Validation errors clearly communicated
   - Swagger documentation accessible

## 3. Prioritized Task Breakdown

### Phase 1: Foundation Setup (Week 1)

#### TASK-001: Complete Repository Implementations
- **Description**: Implement missing repository classes for all entities
- **Dependencies**: None
- **Estimated Effort**: 4 hours
- **Technical Constraints**: Follow existing Repository pattern
- **Acceptance Criteria**: All repositories have CRUD operations
- **Functional Requirements**: Repository<T>, UnitOfWork pattern
- **Quality Assurance**: Unit tests for each repository
- **GitHub Copilot Prompt**: 
  ```
  "I need to implement the missing repository classes (EmailService, DateTimeService, UserRoleRepository, UserRepository, RoleRepository, RolePermissionRepository, PermissionRepository) following the existing Repository pattern. The requirements are implementing IRepository<T> interface with GetByIdAsync, GetAllAsync, GetAsync, Insert, Update, Delete methods. Please help me implement this using Entity Framework Core with the existing SoftwareDeveloperCaseDbContext."
  ```

#### TASK-002: Fix DepartmentRepository GetManagersAsync
- **Description**: Complete the GetManagersAsync method implementation
- **Dependencies**: TASK-001
- **Estimated Effort**: 2 hours
- **Technical Constraints**: Use LINQ with proper joins
- **Acceptance Criteria**: Returns all managers for a department
- **Functional Requirements**: Query users with Manager role in department
- **Quality Assurance**: Unit test with test data
- **GitHub Copilot Prompt**: 
  ```
  "I need to fix the GetManagersAsync method in DepartmentRepository. The requirements are to query all users in a specific department who have the Manager role (RoleId: 9ECA8D57-F7CA-4F8D-9C83-73B659225AE4). Please help me implement this using LINQ with proper joins between Users, UserRoles, and filter by departmentId."
  ```

### Phase 2: Core Services (Week 1-2)

#### TASK-003: Implement EmailService
- **Description**: Create email service for notifications
- **Dependencies**: None
- **Estimated Effort**: 3 hours
- **Technical Constraints**: Use IEmailService interface
- **Acceptance Criteria**: Sends emails via SMTP
- **Functional Requirements**: Configure from appsettings, async operation
- **Quality Assurance**: Integration test with mock SMTP
- **GitHub Copilot Prompt**: 
  ```
  "I need to implement the EmailService class that implements IEmailService. The requirements are sending emails using SMTP with configuration from EmailSettings (ApiKey, FromAddress, FromName), handling the Email model (To, Subject, Body), and implementing proper error handling. Please help me implement this using .NET's SmtpClient or a modern email library."
  ```

#### TASK-004: Implement DateTimeService
- **Description**: Create datetime service for consistent timestamps
- **Dependencies**: None
- **Estimated Effort**: 1 hour
- **Technical Constraints**: Must be testable
- **Acceptance Criteria**: Returns UTC datetime
- **Functional Requirements**: Implements IDateTimeService
- **Quality Assurance**: Unit tests for timezone handling
- **GitHub Copilot Prompt**: 
  ```
  "I need to implement the DateTimeService class that implements IDateTimeService. The requirements are providing a testable way to get the current UTC datetime through the Now property. Please help me implement this as a simple service that returns DateTime.UtcNow."
  ```

### Phase 3: API Completion (Week 2)

#### TASK-005: Add Validation to All Commands
- **Description**: Ensure all command validators are complete
- **Dependencies**: None
- **Estimated Effort**: 4 hours
- **Technical Constraints**: Use FluentValidation
- **Acceptance Criteria**: All inputs validated
- **Functional Requirements**: Email format, required fields, business rules
- **Quality Assurance**: Unit tests for each validator
- **GitHub Copilot Prompt**: 
  ```
  "I need to add comprehensive validation to all command validators (UpdateUserCommandValidator needs email validation, AssignRoleCommand needs validator, AssignPermissionCommand needs validator). The requirements are using FluentValidation to check required fields, email format validation, and ensuring referenced entities exist. Please help me implement this following the existing validator patterns."
  ```

#### TASK-006: Implement Error Handling Middleware
- **Description**: Create global exception handling
- **Dependencies**: None
- **Estimated Effort**: 3 hours
- **Technical Constraints**: Return consistent error format
- **Acceptance Criteria**: All exceptions handled gracefully
- **Functional Requirements**: Log errors, return appropriate status codes
- **Quality Assurance**: Integration tests for various exception types
- **GitHub Copilot Prompt**: 
  ```
  "I need to implement a global exception handling middleware for the API. The requirements are catching all unhandled exceptions, logging them with Serilog, returning consistent error responses with appropriate HTTP status codes (400 for validation, 404 for not found, 500 for server errors), and not exposing internal details in production. Please help me implement this as ASP.NET Core middleware."
  ```

  ##### TASK-006.1: Migrate to ProblemDetails Standard
- **Description**: Update error handling to use RFC 7807 ProblemDetails format
- **Dependencies**: TASK-006
- **Estimated Effort**: 2 hours
- **Technical Constraints**: Must maintain existing error handling behavior
- **Acceptance Criteria**: All error responses follow ProblemDetails format
- **Functional Requirements**: 
  - Replace ErrorResponse model with ProblemDetails
  - Update GlobalExceptionHandlingMiddleware to return RFC 7807 compliant responses
  - Include traceId and instance properties for debugging
  - Configure proper content-type (application/problem+json)
  - Maintain existing status code mapping (400 for validation, 404 for not found, 500 for server errors)
- **Quality Assurance**: 
  - Integration tests verify ProblemDetails format
  - Swagger documentation shows correct error schemas
  - Validate against RFC 7807 specification
- **GitHub Copilot Prompt**: 
  ```
  "I need to migrate the existing error handling from custom ErrorResponse to RFC 7807 ProblemDetails standard. The requirements are updating GlobalExceptionHandlingMiddleware to return ProblemDetails objects, configuring ASP.NET Core ProblemDetails services, ensuring proper JSON serialization with application/problem+json content-type, including traceId for debugging, and maintaining current exception-to-status-code mapping. Please help me implement this migration while preserving existing functionality."
  ```

### Phase 4: Security & Performance (Week 3)

#### TASK-007: Add Input Sanitization
- **Description**: Prevent SQL injection and XSS
- **Dependencies**: TASK-005
- **Estimated Effort**: 3 hours
- **Technical Constraints**: Don't break existing functionality
- **Acceptance Criteria**: All inputs sanitized
- **Functional Requirements**: HTML encoding, SQL parameter binding
- **Quality Assurance**: Security testing suite
- **GitHub Copilot Prompt**: 
  ```
  "I need to add input sanitization to prevent security vulnerabilities. The requirements are sanitizing all string inputs to prevent XSS, ensuring all database queries use parameterized queries (already done with EF Core), and validating input lengths. Please help me implement this using appropriate .NET security libraries and best practices."
  ```

#### TASK-008: Implement Basic Caching
- **Description**: Add caching for frequently accessed data
- **Dependencies**: None
- **Estimated Effort**: 4 hours
- **Technical Constraints**: Use IMemoryCache
- **Acceptance Criteria**: Improved response times
- **Functional Requirements**: Cache permissions, roles, departments
- **Quality Assurance**: Performance tests before/after
- **GitHub Copilot Prompt**: 
  ```
  "I need to implement caching for frequently accessed data like user permissions and roles. The requirements are using IMemoryCache, implementing cache invalidation when data changes, setting appropriate expiration times, and following the decorator pattern. Please help me implement this in the repository layer."
  ```

### Phase 5: Testing & Documentation (Week 3-4)

#### TASK-009: Complete Unit Test Coverage
- **Description**: Achieve 80% code coverage
- **Dependencies**: All previous tasks
- **Estimated Effort**: 8 hours
- **Technical Constraints**: Use xUnit, Moq, FluentAssertions
- **Acceptance Criteria**: 80%+ coverage on business logic
- **Functional Requirements**: Test all commands, queries, services
- **Quality Assurance**: Code coverage reports
- **GitHub Copilot Prompt**: 
  ```
  "I need to create comprehensive unit tests for all command handlers, query handlers, and services. The requirements are using xUnit as the test framework, Moq for mocking dependencies, FluentAssertions for assertions, following AAA pattern, and achieving 80% code coverage. Please help me implement tests for the User and Role features."
  ```

#### TASK-010: API Integration Tests
- **Description**: Test API endpoints end-to-end
- **Dependencies**: TASK-006
- **Estimated Effort**: 6 hours
- **Technical Constraints**: Use WebApplicationFactory
- **Acceptance Criteria**: All endpoints tested
- **Functional Requirements**: Test happy path and error cases
- **Quality Assurance**: CI/CD integration
- **GitHub Copilot Prompt**: 
  ```
  "I need to create integration tests for all API endpoints. The requirements are using WebApplicationFactory for in-memory testing, testing both successful and error scenarios, verifying response formats and status codes, and using a test database. Please help me implement this using xUnit and ASP.NET Core testing utilities."
  ```

### Phase 6: Deployment Preparation (Week 4)

#### TASK-011: Add Health Checks
- **Description**: Implement health check endpoints
- **Dependencies**: None
- **Estimated Effort**: 2 hours
- **Technical Constraints**: Use ASP.NET Core health checks
- **Acceptance Criteria**: Database and service health monitored
- **Functional Requirements**: /health endpoint, detailed checks
- **Quality Assurance**: Manual testing
- **GitHub Copilot Prompt**: 
  ```
  "I need to implement health check endpoints for monitoring. The requirements are adding a /health endpoint that checks database connectivity, email service availability, and overall application health using ASP.NET Core health checks. Please help me implement this with proper health check configuration."
  ```

#### TASK-012: Configuration Management
- **Description**: Improve configuration for different environments
- **Dependencies**: None
- **Estimated Effort**: 3 hours
- **Technical Constraints**: Use appsettings patterns
- **Acceptance Criteria**: Easy environment switching
- **Functional Requirements**: Dev/staging/prod configs
- **Quality Assurance**: Deployment testing
- **GitHub Copilot Prompt**: 
  ```
  "I need to improve configuration management for different environments. The requirements are organizing appsettings for development, staging, and production, using environment variables for sensitive data, implementing IOptions pattern for strongly-typed configuration, and adding configuration validation. Please help me implement this following .NET configuration best practices."
  ```

### Summary Timeline

- **Week 1**: Foundation & Core Services (TASK-001 to TASK-004)
- **Week 2**: API Completion (TASK-005 to TASK-006)
- **Week 3**: Security, Performance & Testing (TASK-007 to TASK-010)
- **Week 4**: Final Testing & Deployment Prep (TASK-011 to TASK-012)

**Total Estimated Effort**: 50 hours (1.25 developer weeks)
**Recommended Team Size**: 1-2 developers
**Critical Path**: TASK-001 → TASK-002 → TASK-005 → TASK-009