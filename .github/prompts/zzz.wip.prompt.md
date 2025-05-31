Reorganize unit tests to mirror source code structure
- Unit test project structure should exactly mirror the layers being tested
- Each source class should have a corresponding test class with "Tests" suffix
- Test folder structure should match source namespace structure
- Example: Domain/Entities/User.cs → Tests/Domain/Entities/UserTests.cs

Create integration test structure
- Separate integration tests by API controllers and infrastructure components
- Use WebApplicationFactory for API integration tests
- Include database integration tests with in-memory provider

## Test Suite Organization Requirements

### Mirror Structure Rules
```
Source: SoftwareDeveloperCase.Domain/Entities/User.cs
Test:   SoftwareDeveloperCase.Test.Unit/Domain/Entities/UserTests.cs

Source: SoftwareDeveloperCase.Application/Commands/Users/CreateUserCommand.cs
Test:   SoftwareDeveloperCase.Test.Unit/Application/Commands/Users/CreateUserCommandTests.cs
```

### Test Categories & Patterns
- **Unit Tests**: Test single units in isolation with mocked dependencies
- **Integration Tests**: Test component interactions with real dependencies
- **Contract Tests**: Test API contracts and data models
- **Behavioral Tests**: Test complete user scenarios end-to-end

### Test Class Structure Template
```csharp
public class {ClassName}Tests
{
    // Test fixtures and mocks setup
    private readonly Mock<IDependency> _mockDependency;
    private readonly {ClassName} _sut; // System Under Test

    public {ClassName}Tests()
    {
        // Constructor setup
    }

    // Group tests by method/functionality
    public class {MethodName}Method : {ClassName}Tests
    {
        [Fact]
        public void {MethodName}_{StateUnderTest}_{ExpectedBehavior}()
        {
            // Arrange
            // Act  
            // Assert
        }
    }
}
```

### Test Naming Conventions
- **Test Classes**: `{SourceClassName}Tests`
- **Test Methods**: `{MethodName}_{StateUnderTest}_{ExpectedBehavior}`
- **Test Files**: Same relative path as source files
- **Nested Classes**: Group tests by method or feature area
- **Test Data**: Use `TheoryData` for complex test cases

### Quality Requirements
- **Coverage**: Minimum 80% for Domain and Application layers
- **Assertions**: Use FluentAssertions for readable assertions
- **Mocking**: Mock all external dependencies, verify interactions
- **Test Data**: Use Object Mother pattern for complex test data creation
- **Performance**: Unit tests should run in <100ms each
- **Independence**: Tests must not depend on execution order

### Test Documentation Standards
- Add XML documentation for complex test scenarios
- Use descriptive test method names that explain the scenario
- Include Given-When-Then comments for complex business logic tests
- Document test data setup and expectations

### Common Test Utilities
- **TestFixtures**: Shared setup for similar test scenarios
- **TestBuilders**: Fluent builders for test data creation
- **TestHelpers**: Common assertion and verification methods
- **TestData**: Shared test data constants and generators

### Integration Test Patterns
- **API Tests**: Use WebApplicationFactory with test database
- **Repository Tests**: Test against real database with transactions
- **Service Tests**: Test with real external service calls (marked as Integration)
- **End-to-End Tests**: Complete user journey testing

### Test Configuration Requirements
- Separate test appsettings.json for test-specific configuration
- Use test-specific dependency injection container setup
- Implement test database cleanup and isolation
- Configure logging for test debugging
- Set up CI/CD test reporting and metrics

### Performance & Reliability
- Parallel test execution where possible
- Deterministic test data to avoid flaky tests
- Proper async/await testing patterns
- Memory leak detection in long-running tests
- Test execution timeout configuration

⏸️ CHECKPOINT: Verify all tests compile and run with proper structure mirroring
- [ ] All test classes follow naming conventions
- [ ] Test folder structure mirrors source code exactly
- [ ] All tests use AAA pattern consistently
- [ ] Test coverage meets minimum requirements
- [ ] Integration tests use proper test fixtures
- [ ] No test dependencies or execution order issues