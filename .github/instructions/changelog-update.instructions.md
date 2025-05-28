---
applyTo:
  - "CHANGELOG.md"
  - "changelog.md"
  - "HISTORY.md"
---

# Changelog Update Instructions

When updating the changelog after completing a validated task, follow these guidelines:

## Structure Requirements

1. **File Format**: Use `CHANGELOG.md` (uppercase) in the project root
2. **Header**: Start with project name and tagline
3. **Unreleased Section**: Always maintain an `[Unreleased]` section at the top
4. **Version Sections**: List versions in reverse chronological order

## Standard Template

```markdown
# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [X.Y.Z] - YYYY-MM-DD

### Added
### Changed
### Deprecated
### Removed
### Fixed
### Security
```

## Entry Guidelines

1. **One Entry Per Change**: Each notable change gets its own bullet point
2. **User Perspective**: Write from the end-user's point of view
3. **Present Tense**: Use present tense for unreleased changes
4. **Past Tense**: Use past tense for released versions
5. **Link References**: Include issue/PR numbers when relevant

## Automation Process

After task completion and validation:
1. Identify the type of change (Added/Changed/Fixed/etc.)
2. Write a concise, user-focused description
3. Add to the appropriate section under `[Unreleased]`
4. Include file paths or components affected if helpful
5. Reference any related issues or pull requests

## Examples by Task Type

### Feature Implementation
```markdown
### Added
- Automated changelog generation using GitHub Copilot prompts
- Support for multiple changelog formats (Markdown, JSON, YAML)
```

### Bug Fix
```markdown
### Fixed
- Changelog entries now properly escape special Markdown characters
- Version comparison links generate correctly for pre-release versions
```

### Refactoring
```markdown
### Changed
- Refactored changelog generation to use template-based approach
- Improved performance of changelog parsing by 40%
```

### Security Update
```markdown
### Security
- Updated dependencies to patch CVE-2024-XXXXX vulnerability
- Implemented input sanitization for user-generated changelog entries
```

## Do's and Don'ts

### Do:
- Keep entries brief and scannable
- Group related changes together
- Mention breaking changes prominently
- Include migration instructions for breaking changes
- Use consistent formatting and style

### Don't:
- Include implementation details unless necessary
- Duplicate information across sections
- Use technical jargon without explanation
- Forget to move entries from Unreleased to versioned sections
- Mix different types of changes in the same section
