# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased] - 2025-05-29

### Fixed
- Resolved compiler warnings for null reference dereferences in validation classes
- Fixed nullability mismatch in user email validation method signature
- Corrected potential null reference access in role name validation logic

### Changed
- Updated all project target frameworks from .NET 6.0 to .NET 9.0
- Converted synchronous role name validation to asynchronous pattern for better performance
- Improved null safety in user command handler for default role assignment

### Added
- Enhanced null safety checks in role and user validation logic
- Proper async/await patterns in FluentValidation validators
