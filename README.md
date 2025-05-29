# SoftwareDeveloperCase

A Clean Architecture implementation for user, role, and permission management system.

## 🏗️ Architecture

This solution follows Clean Architecture principles with the following layers:
- **Domain**: Core business entities and logic
- **Application**: Use cases and business rules
- **Infrastructure**: Data access and external services
- **API**: REST endpoints and HTTP concerns

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

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

# Run tests
dotnet test

# Run the API
dotnet run --project SoftwareDeveloperCase.Api
```


## 🧪 Testing

Run all tests:
```bash
dotnet test
```

Run with coverage:
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## 📚 API Documentation

When running locally, Swagger UI is available at:
```
https://localhost:7001/swagger
```

### Key Endpoints

#### Users
- `POST /api/v1/user` - Register a new user
- `PUT /api/v1/user` - Update user information
- `DELETE /api/v1/user/{id}` - Delete a user
- `GET /api/v1/user/GetUserPermissions/{id}` - Get user permissions
- `POST /api/v1/user/AssignRole` - Assign role to user

#### Roles
- `POST /api/v1/role` - Create a new role
- `POST /api/v1/role/AssignPermission` - Assign permission to role

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 Technical Requirements

<!-- Original requirements preserved for context -->
### Business Rules
- A user has: name, email, department and password
- A user can adopt one or more roles
- There is a hierarchy of roles
- By default there are two roles: Employee and Manager
- Any role will also be an Employee
- Users in a role have certain permissions: read, add, update and delete
- By default a Manager can have all the permissions
- By default an Employee can only have read permission
- Notify the Manager when a user has been registered in their department

### API Requirements
- Register users with email uniqueness validation
- Register roles with hierarchy support
- Assign permissions to roles
- Assign roles to users
- Get user permissions based on roles

