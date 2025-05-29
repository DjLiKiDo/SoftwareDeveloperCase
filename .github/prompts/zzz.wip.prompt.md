Quiero empezar a implementar funcionalidad real en mi api. Vamos a crear una api para un software de gestión de proyectos que permita a los usuarios gestionar equipos, proyectos y tareas. La API debe ser segura, escalable y fácil de mantener.


**Architecture & Technical Requirements**
- Implement Clean Architecture patterns with separate layers (API, Core, Infrastructure)
- Use Entity Framework Core 8 for data persistence
- Follow RESTful API best practices and SOLID principles
- Implement comprehensive exception handling and logging
- Include OpenAPI/Swagger documentation
- Use FluentValidation for request validation

**Authentication & Authorization**
- Implement JWT-based authentication with refresh token support
- Configure role-based authorization with specified roles: Admin, Manager, Developer
- Implement password hashing using industry-standard algorithms
- Include rate limiting for API endpoints

**Domain Models**

1. User
```csharp
public class User {
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public Role Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
```

2. Team
```csharp
public class Team {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int MaxCapacity { get; set; }
    public ICollection<TeamMember> Members { get; set; }
    public ICollection<Project> Projects { get; set; }
}
```

3. TeamMember
```csharp
public class TeamMember {
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid TeamId { get; set; }
    public TeamRole Role { get; set; }
    public DateTime JoinDate { get; set; }
    public MemberStatus Status { get; set; }
}
```

4. Project
```csharp
public class Project {
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ProjectStatus Status { get; set; }
    public Guid TeamId { get; set; }
    public ICollection<Task> Tasks { get; set; }
}
```

5. Task
```csharp
public class Task {
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Priority Priority { get; set; }
    public DateTime DueDate { get; set; }
    public TaskStatus Status { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? AssigneeId { get; set; }
    public Guid? ParentTaskId { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public ICollection<Task> SubTasks { get; set; }
    public ICollection<TaskComment> Comments { get; set; }
    public ICollection<TaskAttachment> Attachments { get; set; }
}
```
PArtiendo de esta idea, vamos a modificar nuestro proyecto para adaptarlo a estos requisitos que te paso como referencia, sugiere lo que creas conveniente para un MVP



El objetivo es crear la documentacion necesaria (PRD, MVP, BOARD) para implementar esta fase del proyecto, con una especificacion clara y una lista 

Como tarea antes de comenzar la implementacion, hacer un analisis de deuda tecnica y añadir las tareas necesarias para abordar los problemas identificados debidamente priorizadas.
Como tarea al finalizar y antes de desplegar el mvp, hacer un analisis de deuda tecnica y añadir las tareas necesarias para abordar los problemas identificados debidamente priorizadas.

Each task should follow this format:
- Task ID
- Description 
- Priority
- Dependencies
- Estimated effort
- Technical constraints
- Acceptance Criteria
- Functional Requirements
- Quality Assurance
- GitHub Copilot prompt template: 
  "I need to [task objective]. The requirements are [specific details]. Please help me implement this using [technology/framework]."
  

