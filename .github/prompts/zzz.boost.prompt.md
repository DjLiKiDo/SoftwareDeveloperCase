# Project Management API - MVP Development Prompt

## Context
Transform the existing SoftwareDeveloperCase solution into a comprehensive Project Management API that enables teams to manage projects and tasks efficiently.

## Objectives
1. **Primary Goal**: Create an MVP for a project management system
2. **Secondary Goals**: 
   - Document technical debt before and after implementation
   - Create comprehensive project documentation (PRD, MVP scope, Task Board)
   - Define clear implementation tasks with GitHub Copilot prompts

## Core Requirements

### Functional Requirements
- **User Management**: Registration, authentication, profile management
- **Team Management**: Create, join, leave teams with capacity limits
- **Project Management**: CRUD operations with status tracking
- **Task Management**: Hierarchical tasks with assignments and time tracking
- **Collaboration**: Comments and attachments on tasks

### Technical Constraints
- Must reuse existing Clean Architecture structure
- Leverage current authentication/authorization system
- Maintain backward compatibility where possible
- Follow established coding standards and patterns

## Domain Model Enhancements

### Enums Definition
```csharp
public enum Role { Admin, Manager, Developer }
public enum TeamRole { Leader, Member }
public enum MemberStatus { Active, Inactive, OnLeave }
public enum ProjectStatus { Planning, Active, OnHold, Completed, Cancelled }
public enum TaskStatus { Todo, InProgress, InReview, Done, Blocked }
public enum Priority { Low, Medium, High, Critical }