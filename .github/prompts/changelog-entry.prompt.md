# Generate Changelog Entry

After completing and validating a task, generate a changelog entry following the [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) format.

## Input Requirements
- Task description: [TASK_DESCRIPTION]
- Change type: [CHANGE_TYPE] (Added/Changed/Deprecated/Removed/Fixed/Security)
- Affected files: [FILES_CHANGED]
- Version: [VERSION] (or "Unreleased" if not yet released)
- Date: [DATE] (format: YYYY-MM-DD)

## Changelog Entry Format

```markdown
## [VERSION] - DATE

### CHANGE_TYPE
- Brief, user-focused description of the change
- Additional details if necessary
- Reference to issue/PR if applicable (#XXX)
```

## Guidelines for Writing Entries

1. **Be User-Focused**: Describe what changed from the user's perspective, not implementation details
2. **Be Concise**: One line per change, expand only if necessary
3. **Be Consistent**: Use the same tense and style throughout
4. **Group by Type**: Place the entry under the correct change type section

## Change Type Definitions
- **Added**: New features or functionality
- **Changed**: Changes to existing functionality
- **Deprecated**: Features that will be removed in future versions
- **Removed**: Features that have been removed
- **Fixed**: Bug fixes
- **Security**: Security vulnerability fixes

## Examples

### For a New Feature:
```markdown
### Added
- GitHub Copilot changelog automation prompt template for consistent documentation
```

### For a Bug Fix:
```markdown
### Fixed
- Corrected file path validation in prompt templates to prevent generation errors
```

### For Multiple Changes:
```markdown
### Added
- Changelog entry generation prompt template
- Automatic version detection from git tags

### Changed
- Updated documentation structure to follow Keep a Changelog format
- Improved prompt variable naming for better clarity
```

## Output Requirements
1. Generate the changelog entry in the correct format
2. Place it in the appropriate section of CHANGELOG.md
3. If CHANGELOG.md doesn't exist, create it with proper structure
4. Maintain reverse chronological order (newest first)
5. Include comparison links for versions if using GitHub
