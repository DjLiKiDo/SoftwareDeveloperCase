## Implementation Plan

This plan addresses the request to restructure and optimize the codebase, focusing on the structural aspects first as per the prompt. User feedback has been incorporated.

### Phase 1: Preparation and Clarification
- [x] Initial analysis of the request and existing workspace structure.
- [x] Formulate clarifying questions for the user.
- [x] User feedback received and incorporated.
- [x] **WAIT** for user approval of the updated plan before proceeding to Phase 2. (Approved)

### Phase 2: One Class Per File
- [ ] Scan the codebase (all C# projects: Api, Application, Domain, Infrastructure) for files containing multiple class, struct, interface, or enum definitions.
- [ ] For each identified file:
    - [ ] Create new files for each additional type, naming the file after the type (e.g., `MyClass.cs` for `class MyClass`).
    - [ ] Move the type definition to its new file.
    - [ ] Ensure the namespace in the new file is correct (initially, it will be the same as the original file; will be updated in Phase 4).
    - [ ] Update using statements in the original and other affected files if necessary.
- [ ] ⏸️ **CHECKPOINT**: Request human to compile the solution (`dotnet build`) and confirm no functionality is broken. All tests should pass (`dotnet test`).

### Phase 3: Directory Structure Reorganization
This phase implements the agreed-upon directory structure.

**Domain Layer (`SoftwareDeveloperCase.Domain/`) Proposal:**
- `Entities/`
    - `User.cs`
    - `Role.cs`
    - `Team/` (folder for Team-related entities)
        - `Team.cs`
        - `TeamMember.cs`
    - `Project/` (folder for Project-related entities)
        - `Project.cs`
    - `Task/` (folder for Task-related entities)
        - `Task.cs`
        - `TaskComment.cs`
- `Enums/`
    - `ProjectStatus.cs`
    - `TaskStatus.cs`
    - `Priority.cs`
    - `RoleType.cs` (or similar, for User Roles)
    - `TeamRole.cs`
    - `MemberStatus.cs`
- `ValueObjects/` (Organized by entity if specific, e.g., `ValueObjects/Users/Address.cs`, or general)
- `Events/` (Organized by entity if specific, e.g., `Events/Users/UserCreatedEvent.cs`, or general)
- `Interfaces/`
    - `Repositories/`
        - `IUserRepository.cs`
        - `IProjectRepository.cs`
        - `ITaskRepository.cs`
        - `ITeamRepository.cs`
        - `IRoleRepository.cs` // Added for completeness
    - `Services/` (Domain services, if any)
- `Common/` (Base classes like AuditableEntity, common domain logic)

**Application Layer (`SoftwareDeveloperCase.Application/`) Proposal:
- `Features/`
    - `Users/`
        - `Commands/`
            - `CreateUser/`
                - `CreateUserCommand.cs`
                - `CreateUserCommandHandler.cs`
                - `CreateUserCommandValidator.cs` (moved here)
        - `Queries/`
            - `GetUserById/`
                - `GetUserByIdQuery.cs`
                - `GetUserByIdQueryHandler.cs`
        - `DTOs/`
            - `UserDto.cs`
            - `UserDetailsDto.cs`
    - `Projects/` (similar structure: Commands, Queries, DTOs, with validators in command/query folders)
    - `Tasks/` (similar structure)
    - `Teams/` (similar structure)
    - `Roles/` (similar structure)
- `Contracts/`
    - `Infrastructure/` (Interfaces for infrastructure services, e.g., `IEmailService.cs`, `IFileStorageService.cs`)
    - `Identity/` (e.g. `IAuthenticationService.cs` if app layer defines it)
- `Mappings/`
    - `UserProfile.cs`
    - `ProjectProfile.cs`
    - `TaskProfile.cs` (AutoMapper profiles, one per main entity/feature)
- `Behaviours/` (MediatR pipeline behaviours)
- `Exceptions/` (Custom application exceptions)
- `Models/` (General application models, if any, not DTOs or Commands/Queries)

**Infrastructure Layer (`SoftwareDeveloperCase.Infrastructure/`) Proposal:**
- `Persistence/`
    - `SqlServer/` (or current/default provider name)
        - `Repositories/`
            - `UserRepository.cs`
            - `ProjectRepository.cs`
            - `TaskRepository.cs`
            - `TeamRepository.cs`
            - `RoleRepository.cs` // Added for completeness
        - `DbContext/`
            - `ApplicationDbContext.cs`
        - `Migrations/` (existing)
        - `Configurations/` (EF Core Fluent API configurations for entities)
            - `UserConfiguration.cs`
            - `ProjectConfiguration.cs`
    - `MongoDb/` (Example, if added later)
        - `Repositories/`
- `Identity/`
    - `Services/`
        - `AuthenticationService.cs` (Implementation of `IAuthenticationService` if JWT logic is here)
    - `Models/` (e.g. `JwtSettings.cs`)
- `ExternalServices/`
    - `Email/`
        - `SmtpEmailService.cs` (Implementation of `IEmailService`)
    - `FileStorage/` (Example)
        - `AzureBlobStorageService.cs`
- `Caching/`
    - `Redis/`
        - `RedisCacheService.cs`
- `Services/` (Folder for other general infrastructure service implementations, if not fitting above categories)

**API Layer (`SoftwareDeveloperCase.Api/`) Proposal:**
- `Controllers/`
    - `V1/`
        - `UsersController.cs`
        - `ProjectsController.cs`
        - `TasksController.cs`
        - `TeamsController.cs`
        - `RolesController.cs`
- `Middleware/` (existing)
- `Filters/` (existing)
- `Models/` (API-specific request/response models if not covered by Application DTOs, e.g. for `Result<T>`)
    - `ApiResponse.cs` (if using a generic wrapper)
- `HealthChecks/` (existing)
- `Extensions/` (e.g., for `IServiceCollection` extensions specific to API setup)

**Steps for Phase 3:**
- [x] For each layer (Domain, Application, Infrastructure, API), create the new directory structures as proposed above.
- [x] Move existing files to their new locations according to the new structure.
    - [x] Domain: Organized entities into Team/, Project/, Task/ subdirectories
    - [x] Application: Moved DTOs into Features/{Feature}/DTOs/ folders
    - [x] Infrastructure: Organized into Persistence/SqlServer/ structure with proper subdirectories
    - [x] API: Organized controllers into V1/ and moved request/response models to Models/
- [x] Delete old/empty directories after moving files.
- [x] ⏸️ **CHECKPOINT**: Request human to review the new directory structure. Compile the solution (`dotnet build`) and confirm no functionality is broken. All tests should pass (`dotnet test`). (Note: Namespaces will be fixed in Phase 4, so build errors due to namespaces are expected at this stage if not preemptively updated, but the primary check is file organization).

### Phase 4: Namespace Organization
- [x] Analyze all C# files to identify namespace mismatches with directory structure
- [x] Identified 20+ files that need namespace updates across Infrastructure, API, and Domain layers
- [x] Update Infrastructure layer namespaces (Migration files fixed)
- [x] Update API layer namespaces (Controller and model namespaces aligned)  
- [x] Update Domain layer namespaces (Role entity relocated)
- [x] Update using statements across the codebase to reflect new namespaces
- [x] Fixed EmailService namespace conflicts (Email namespace vs Email type)
- [x] Resolved DbContext namespace conflicts 
- [x] Human made manual edits to repository files and migration designers
- [ ] Fix remaining 3 DependencyInjection errors (DbContext and Services references)
- [ ] ⏸️ **CHECKPOINT**: Request human to compile the solution and confirm no functionality is broken. All tests should pass.

### Phase 5: Review Dependency Injection and SOLID Principles
- [ ] Review existing `DependencyInjection.cs` files in Api, Application, and Infrastructure layers. Ensure they align with the new structure and SOLID principles.
- [ ] Ensure DI registrations correctly point to types in their new namespaces/locations.
- [ ] During all refactoring steps, apply SOLID principles where appropriate (e.g., ensuring classes have a single responsibility after being split).
- [ ] Focus on ensuring DI is correctly used and its configuration is aligned with the new structure.
- [ ] ⏸️ **CHECKPOINT**: Request human to review DI setup and overall structure. Compile and test.

### Phase 6: Final Review and Cleanup
- [ ] Run `dotnet format` on the entire solution.
- [ ] Ensure all tests pass (`dotnet test`).
- [ ] Review changes for adherence to all project instructions and coding standards.
- [ ] **WAIT** for final human approval.

## Current Status
- ✅ Completed: Phase 1 - Preparation and Clarification. User feedback incorporated. Plan approved.
- ✅ Completed: Phase 2 - One Class Per File. All multiple-type files have been extracted into individual files.
- ✅ Completed: Phase 3 - Directory Structure Reorganization. All layers reorganized according to approved structure.
- ✅ Completed: Human checkpoint review. Ready to proceed with namespace updates.
- ⚠️ In progress: Phase 4 - Namespace Organization
  - ✅ Fixed migration namespace issues 
  - ✅ Fixed API layer namespace conflicts
  - ✅ Fixed Domain layer Role entity namespace
  - ✅ Resolved EmailService and DbContext namespace conflicts
  - ✅ Human completed manual edits to repository files
  - ⚠️ **Current**: 3 remaining compilation errors in DependencyInjection.cs (DbContext and Services references)
- ⏳ Pending: Phases 5-6.

## Remaining Work for Phase 4
The build is nearly successful with only 3 errors remaining in `/src/SoftwareDeveloperCase.Infrastructure/DependencyInjection.cs`:
1. `SoftwareDeveloperCaseDbContext` not found in `SoftwareDeveloperCase.Infrastructure.Persistence` namespace
2. `DateTimeService` could not be found
3. `CacheKeyService` could not be found

These need to be fixed to complete Phase 4 before proceeding to Phase 5.
