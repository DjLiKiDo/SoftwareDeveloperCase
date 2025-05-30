# SoftwareDeveloperCase - Project Management API

A comprehensive Clean Architecture implementation for team-based project and task management system.

## 🎯 Project Overview

This API provides a robust solution for managing teams, projects, and tasks with hierarchical organization, time tracking, and collaboration features. Built with Clean Architecture principles, it ensures maintainability, testability, and scalability.

## 🏗️ Architecture

This solution follows Clean Architecture principles with the following layers:
- **Domain**: Core business entities, enums, and business rules
- **Application**: Use cases, DTOs, validators, and business logic
- **Infrastructure**: Data persistence, external services, and identity
- **API**: RESTful endpoints, middleware, and HTTP concerns

### Key Patterns
- **CQRS**: Command Query Responsibility Segregation using MediatR
- **Repository Pattern**: With Unit of Work for data access
- **Domain-Driven Design**: Rich domain models with business logic
- **Dependency Injection**: IoC container for loose coupling

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code
- Git

### Installation
```bash
# Clone the repository
git clone https://github.com/yourusername/SoftwareDeveloperCase.git

# Navigate to the solution directory
cd SoftwareDeveloperCase

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run database migrations
dotnet ef database update -p SoftwareDeveloperCase.Infrastructure -s SoftwareDeveloperCase.Api

# Run tests
dotnet test

# Run the API
dotnet run --project SoftwareDeveloperCase.Api
```

### Quick Start
1. Register a new user via POST `/api/v1/auth/register`
2. Login to get JWT token via POST `/api/v1/auth/login`
3. Use the token in Authorization header: `Bearer {token}`
4. Create a team, project, and start managing tasks!

## 🧪 Testing

Run all tests:
```bash
dotnet test
```

Run with coverage:
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

Run specific test category:
```bash
dotnet test --filter Category=Unit
dotnet test --filter Category=Integration
```

## 📚 API Documentation

When running locally, Swagger UI is available at:
```
https://localhost:7001/swagger
```

### Core Endpoints

#### Authentication
- `POST /api/v1/auth/register` - Register new user
- `POST /api/v1/auth/login` - Login with credentials
- `POST /api/v1/auth/refresh` - Refresh access token
- `POST /api/v1/auth/logout` - Logout and revoke tokens

#### Teams
- `GET /api/v1/teams` - List all teams (paginated)
- `GET /api/v1/teams/{id}` - Get team details
- `POST /api/v1/teams` - Create new team (Manager/Admin)
- `PUT /api/v1/teams/{id}` - Update team
- `DELETE /api/v1/teams/{id}` - Delete team
- `POST /api/v1/teams/{id}/members` - Add team member
- `DELETE /api/v1/teams/{id}/members/{userId}` - Remove member

#### Projects
- `GET /api/v1/projects` - List projects (filterable)
- `GET /api/v1/projects/{id}` - Get project details
- `POST /api/v1/projects` - Create new project
- `PUT /api/v1/projects/{id}` - Update project
- `DELETE /api/v1/projects/{id}` - Delete project
- `PATCH /api/v1/projects/{id}/status` - Update status

#### Tasks
- `GET /api/v1/tasks` - List tasks (filterable)
- `GET /api/v1/tasks/{id}` - Get task details
- `POST /api/v1/tasks` - Create new task
- `PUT /api/v1/tasks/{id}` - Update task
- `DELETE /api/v1/tasks/{id}` - Delete task
- `POST /api/v1/tasks/{id}/assign` - Assign task
- `POST /api/v1/tasks/{id}/time` - Log time
- `POST /api/v1/tasks/{id}/comments` - Add comment
- `GET /api/v1/tasks/{id}/comments` - Get comments

## 🏛️ Domain Model

### Core Entities
- **User**: System users with authentication
- **Team**: Groups of users working together
- **TeamMember**: User membership in teams
- **Project**: Work containers with timelines
- **Task**: Work items with hierarchy support
- **TaskComment**: Collaboration on tasks

### Enumerations
- **Role**: Admin, Manager, Developer
- **TeamRole**: Leader, Member
- **MemberStatus**: Active, Inactive, OnLeave
- **ProjectStatus**: Planning, Active, OnHold, Completed, Cancelled
- **TaskStatus**: Todo, InProgress, InReview, Done, Blocked
- **Priority**: Low, Medium, High, Critical

## 🔒 Security

- JWT Bearer authentication with refresh tokens
- Role-based authorization (Admin, Manager, Developer)
- Rate limiting per user/IP
- Input validation and sanitization
- HTTPS enforcement
- Security headers implementation

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Make your changes following coding standards
4. Write/update tests as needed
5. Update documentation
6. Commit your changes (`git commit -m 'Add AmazingFeature'`)
7. Push to the branch (`git push origin feature/AmazingFeature`)
8. Open a Pull Request

### Coding Standards
- Follow Microsoft C# naming conventions
- Use file-scoped namespaces
- Write XML documentation for public APIs
- Maintain 80% test coverage minimum
- Run `dotnet format` before committing

## 📊 Project Status

**Current Version**: 1.0.0-preview (MVP)

### Completed Features ✅
- User authentication and authorization
- Team management
- Project lifecycle management
- Task management with hierarchy
- Basic time tracking
- Task comments

### Upcoming Features 🚧
- Email notifications
- File attachments
- Advanced reporting
- Real-time updates
- Third-party integrations

## 📝 Technical Requirements

### Business Rules
- Users belong to one or more teams
- Teams have capacity limits (default: 10)
- Projects belong to a single team
- Tasks can have subtasks (one level)
- Time tracking is manual entry
- All users can comment on accessible tasks

### Performance Targets
- API response time < 300ms (MVP)
- Support 100 concurrent users (MVP)
- 95% uptime SLA

## 🔧 Configuration

### Environment Variables
```bash
# Database
ConnectionStrings__DefaultConnection="Server=...;Database=...;Trusted_Connection=true;"

# JWT
Jwt__Secret="your-secret-key-min-32-chars"
Jwt__Issuer="your-issuer"
Jwt__Audience="your-audience"
Jwt__ExpirationMinutes="15"

# Logging
Serilog__MinimumLevel__Default="Information"
```

### appsettings.json Structure
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "ExpirationMinutes": 15,
    "RefreshExpirationDays": 7
  }
}
```

## 🐛 Troubleshooting

### Common Issues

1. **Database Connection Failed**
   - Verify SQL Server is running
   - Check connection string
   - Ensure database exists

2. **JWT Token Invalid**
   - Check token expiration
   - Verify secret key matches
   - Ensure bearer format

3. **Migration Errors**
   - Delete existing migrations
   - Recreate with `dotnet ef migrations add Initial`
   - Update database

## 🔄 CI/CD Pipeline

This project includes a comprehensive GitHub Actions workflow that provides:

### Features
- **Multi-stage Testing**: Unit tests, integration tests, and performance tests
- **Code Quality**: Format verification, vulnerability scanning, and code coverage
- **Security**: Dependency vulnerability checks and CodeQL analysis
- **Containerization**: Docker image building and publishing to GitHub Container Registry
- **Deployment**: Automated staging and production deployment support
- **Artifacts**: Test results, coverage reports, and build artifacts

### Workflow Jobs

1. **Test & Code Quality**: 
   - Runs unit tests with coverage reporting
   - Verifies code formatting with `dotnet format`
   - Scans for vulnerable and deprecated packages
   - Uploads test results and coverage reports

2. **Build & Package**:
   - Builds release artifacts
   - Publishes API for deployment
   - Uploads build artifacts

3. **Integration Tests**:
   - Runs with SQL Server container
   - Tests full application stack
   - Validates database interactions

4. **Performance Tests**:
   - Basic load testing framework
   - API health checks
   - Performance baseline validation

5. **Docker**:
   - Builds and pushes container images
   - Uses GitHub Container Registry (ghcr.io)
   - Implements proper caching strategies

6. **Deployment**:
   - Staging deployment (develop branch)
   - Production deployment (main branch)
   - Environment-specific configurations

### Configuration

#### Required Secrets (Optional)
- `CODECOV_TOKEN`: For Codecov integration (optional)
- `DOCKER_USERNAME`/`DOCKER_PASSWORD`: For Docker Hub (optional, uses GHCR by default)

#### GitHub Environments
To enable deployment protection rules, configure these environments in your repository:
- `staging`: For develop branch deployments
- `production`: For main branch deployments

#### Workflow Triggers
- **Push**: `main` and `develop` branches
- **Pull Request**: `main` and `develop` branches  
- **Release**: Published releases
- **Manual**: `workflow_dispatch` for manual runs

### Local Docker

```bash
# Build Docker image
docker build -t softwaredevelopercase:latest -f SoftwareDeveloperCase.Api/Dockerfile .

# Run container
docker run -p 8080:8080 -p 8081:8081 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  softwaredevelopercase:latest
```

### Code Quality Standards

The CI pipeline enforces:
- **Code Coverage**: Minimum 80% for Domain/Application layers
- **Code Formatting**: Consistent style with `dotnet format`
- **Security**: No vulnerable dependencies
- **Testing**: All tests must pass
- **Build**: Clean compilation without warnings

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with Clean Architecture principles
- Inspired by Domain-Driven Design
- Following Microsoft best practices
- Community feedback and contributions

---

**Documentation**: [/docs](./docs)  
**API Version**: v1  
**Last Updated**: May 2025

