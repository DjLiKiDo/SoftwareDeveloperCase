# Dependency Organization Documentation

## Overview

This document outlines the systematic reorganization of project dependencies across the SoftwareDeveloperCase solution to improve maintainability, readability, and adherence to Clean Architecture principles.

## Changes Made

### 1. Project File Structure Standardization

All `.csproj` files now follow a consistent structure:

```xml
<!-- Project Configuration -->
<PropertyGroup>
  <!-- Build settings -->
</PropertyGroup>

<!-- Assembly Visibility Configuration -->
<ItemGroup>
  <!-- InternalsVisibleTo attributes if needed -->
</ItemGroup>

<!-- Internal Project References -->
<ItemGroup>
  <!-- ProjectReference elements -->
</ItemGroup>

<!-- External Package Dependencies -->
<ItemGroup>
  <!-- PackageReference elements grouped by purpose with comments -->
</ItemGroup>

<!-- Configuration Files -->
<ItemGroup>
  <!-- Content files and other items -->
</ItemGroup>
```

### 2. Language Version Update

- Updated all projects from `LangVersion 12` to `LangVersion 13` to use the latest C# features
- Maintained consistency across all projects

### 3. Dependency Categorization

Dependencies are now organized with descriptive comments:

#### SoftwareDeveloperCase.Domain

- **Clean Architecture Principle**: No external dependencies
- Only core .NET runtime dependencies

#### SoftwareDeveloperCase.Application

- **Object Mapping**: AutoMapper
- **Cryptography & Security**: BCrypt.Net-Next, HtmlSanitizer
- **Validation**: FluentValidation packages
- **CQRS & Mediator Pattern**: MediatR
- **Logging Abstractions**: Microsoft.Extensions.Logging.Abstractions

#### SoftwareDeveloperCase.Infrastructure

- **Database & Entity Framework**: EF Core packages
- **Configuration & Options**: Microsoft Extensions packages

#### SoftwareDeveloperCase.Api

- **Authentication & Authorization**: JWT Bearer packages
- **Database Tooling**: EF Core Design tools
- **Health Checks**: EF Core health checks
- **Logging & Monitoring**: Serilog packages
- **API Documentation**: Swagger/Swashbuckle packages

#### Test Projects

- **Core Testing Framework**: xUnit, Microsoft.NET.Test.Sdk
- **Code Coverage**: coverlet.collector
- **Testing Utilities**: FluentAssertions, Moq
- **Integration Testing**: ASP.NET Core Testing, TestContainers, EF InMemory
- **Security Updates**: BouncyCastle, System.Net.Http, System.Text.RegularExpressions

### 4. Package Version Alignment

Updated package versions for consistency:

| Package                   | Previous     | Updated |
| ------------------------- | ------------ | ------- |
| Microsoft.NET.Test.Sdk    | 17.11.1      | 17.14.0 |
| xunit                     | 2.9.2        | 2.9.3   |
| xunit.runner.visualstudio | 2.8.2/3.1.0  | 3.1.0   |
| coverlet.collector        | 6.0.2/6.0.4  | 6.0.4   |
| FluentAssertions          | 6.12.2/8.3.0 | 8.3.0   |

### 5. Test Project Improvements

- Added missing `IsPackable` and `IsTestProject` properties
- Standardized package versions across both test projects
- Improved organization of test-specific dependencies

## Benefits

1. **Improved Readability**: Dependencies are clearly categorized and commented
2. **Easier Maintenance**: Consistent structure makes updates and reviews simpler
3. **Version Consistency**: All packages use the latest compatible versions
4. **Clean Architecture**: Dependencies respect architectural boundaries
5. **Better Understanding**: Comments explain the purpose of each dependency group

## Architectural Compliance

The reorganization maintains Clean Architecture principles:

- **Domain**: No external dependencies ✅
- **Application**: Only depends on Domain ✅
- **Infrastructure**: Depends on Application and Domain ✅
- **API**: References Application and Infrastructure ✅
- **Tests**: Can reference all layers for testing purposes ✅

## Security Improvements

- Updated security-sensitive packages in test projects
- Maintained explicit security vulnerability fixes
- Ensured all packages use latest stable versions

## Future Maintenance

To maintain this organization:

1. **New Dependencies**: Add to appropriate category with descriptive comments
2. **Version Updates**: Update consistently across all projects
3. **Regular Reviews**: Quarterly dependency audits
4. **Documentation**: Update this document when making structural changes

## Verification

All changes have been verified:

- ✅ Solution restores successfully
- ✅ All projects build without errors
- ✅ Dependencies are properly resolved
- ✅ Test projects maintain all required references
