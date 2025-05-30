# CI/CD Pipeline Documentation

## Overview

This project uses GitHub Actions for continuous integration and deployment. The pipeline is designed to ensure code quality, security, and reliable deployments.

## Pipeline Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Pull Request  │    │   Push to Dev   │    │  Push to Main   │
│                 │    │                 │    │                 │
└─────────┬───────┘    └─────────┬───────┘    └─────────┬───────┘
          │                      │                      │
          ▼                      ▼                      ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  Test & Build   │    │  Test & Build   │    │  Test & Build   │
│                 │    │                 │    │                 │
└─────────┬───────┘    └─────────┬───────┘    └─────────┬───────┘
          │                      │                      │
          ▼                      ▼                      ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│ Integration     │    │ Docker Build    │    │ Docker Build    │
│ Tests           │    │                 │    │                 │
└─────────────────┘    └─────────┬───────┘    └─────────┬───────┘
                                 │                      │
                                 ▼                      ▼
                       ┌─────────────────┐    ┌─────────────────┐
                       │Deploy to Staging│    │Deploy to Prod   │
                       │                 │    │                 │
                       └─────────────────┘    └─────────────────┘
```

## Workflow Files

### Main Workflow: `.github/workflows/ci-cd.yml`
Primary CI/CD pipeline with comprehensive testing and deployment.

### Security Workflows:
- `.github/workflows/codeql.yml`: GitHub CodeQL security analysis
- `.github/workflows/dependency-review.yml`: Dependency vulnerability scanning for PRs

### Automation:
- `.github/renovate.json`: Automated dependency updates with Renovate

## Job Details

### 1. Test & Code Quality
**Triggers**: All pushes and PRs  
**Duration**: ~3-5 minutes  
**Purpose**: Ensure code quality and functionality

**Steps**:
- Code checkout with full history
- .NET 8 SDK setup with NuGet caching
- Dependency restoration
- Code format verification (`dotnet format --verify-no-changes`)
- Solution build (Release configuration)
- Unit test execution with coverage collection
- Coverage report generation using ReportGenerator
- Test results upload as artifacts
- Coverage upload to Codecov (if token provided)
- Vulnerability scanning for packages
- Deprecated package detection

**Artifacts Generated**:
- Test results (TRX format)
- Code coverage reports (HTML, Cobertura, JSON)
- Vulnerability reports

### 2. Build & Package
**Triggers**: Push events (not PRs)  
**Duration**: ~2-3 minutes  
**Purpose**: Create deployable artifacts

**Steps**:
- Checkout and .NET setup
- Dependency restoration with caching
- Release build with version numbering
- API project publishing
- Artifact upload for deployment

**Artifacts Generated**:
- Published API binaries
- Configuration files
- Static assets

### 3. Integration Tests
**Triggers**: All pushes and PRs  
**Duration**: ~5-8 minutes  
**Purpose**: Test full application stack

**Features**:
- SQL Server 2022 service container
- Health check validation
- Database connection testing
- Full application stack validation

**Steps**:
- SQL Server container startup
- Connection health verification
- Integration test execution
- Test result collection

### 4. Performance Tests
**Triggers**: Push to main branch only  
**Duration**: ~3-5 minutes  
**Purpose**: Baseline performance validation

**Steps**:
- API startup in background
- Basic load testing with curl
- Performance metric collection
- API shutdown

### 5. Docker Build & Push
**Triggers**: Push to main/develop branches  
**Duration**: ~4-6 minutes  
**Purpose**: Container image creation

**Features**:
- Multi-stage Docker build
- GitHub Container Registry integration
- Build cache optimization
- Metadata extraction for tags

**Image Tags**:
- Branch name (e.g., `main`, `develop`)
- Git SHA prefix (e.g., `main-abc1234`)
- `latest` for main branch

### 6. Deployment Jobs
**Triggers**: After successful Docker build  
**Duration**: ~1-2 minutes per environment  
**Purpose**: Environment-specific deployments

**Staging** (develop branch):
- Automated deployment
- Environment validation
- Smoke tests

**Production** (main branch):
- Manual approval (when environments configured)
- Blue-green deployment ready
- Health checks

## Configuration

### Environment Variables
Set in workflow file:
```yaml
env:
  DOTNET_VERSION: '8.0.x'
  SOLUTION_FILE: 'SoftwareDeveloperCase.sln'
  DOTNET_NOLOGO: true
  DOTNET_CLI_TELEMETRY_OPTOUT: true
```

### Repository Secrets (Optional)
- `CODECOV_TOKEN`: For enhanced coverage reporting
- `DOCKER_USERNAME`: For Docker Hub integration
- `DOCKER_PASSWORD`: For Docker Hub integration

### Repository Variables
Configure in GitHub repository settings:
- `DOCKER_ENABLED`: Set to 'true' to enable Docker builds
- `DOCKER_REGISTRY`: 'dockerhub' or 'ghcr' (defaults to ghcr)

### GitHub Environments
For deployment protection rules:

1. **Staging Environment**:
   - Branch protection: `develop`
   - Auto-deployment enabled
   - Required reviewers: 0

2. **Production Environment**:
   - Branch protection: `main`
   - Manual approval required
   - Required reviewers: 1+

## Security Features

### Vulnerability Scanning
- **Package Vulnerabilities**: Scans for known security issues in NuGet packages
- **Deprecated Packages**: Identifies outdated dependencies
- **CodeQL Analysis**: Static code analysis for security patterns
- **Dependency Review**: PR-based dependency change analysis

### Container Security
- **Non-root User**: Docker containers run as unprivileged user
- **Minimal Base Image**: Uses official Microsoft .NET images
- **Multi-stage Build**: Reduces final image size and attack surface

### Secrets Management
- **GitHub Secrets**: Secure storage for sensitive data
- **Environment Protection**: Production deployments require approval
- **Token Permissions**: Least privilege access for workflows

## Monitoring & Alerts

### Success Metrics
- Build success rate: >95%
- Test coverage: >80%
- Performance regression: <10%
- Security vulnerabilities: 0

### Failure Notifications
- Build failures notify via GitHub
- Coverage drops trigger warnings
- Security vulnerabilities fail builds
- Performance regressions logged

## Troubleshooting

### Common Issues

**Build Failures**:
1. Check NuGet package conflicts
2. Verify .NET version compatibility
3. Review dependency versions
4. **NuGet Lock File Error**: If seeing "Dependencies lock file is not found", this indicates the built-in `cache: true` setting is looking for `packages.lock.json`. Our workflow uses custom NuGet caching for better control.

**Test Failures**:
1. Check SQL Server connectivity
2. Verify test database setup
3. Review integration test configuration

**Docker Build Issues**:
1. Verify Dockerfile syntax
2. Check .dockerignore file
3. Review build context size

**Deployment Failures**:
1. Check environment configuration
2. Verify secrets and variables
3. Review deployment scripts

### Debug Steps
1. Check workflow logs in GitHub Actions
2. Review artifact outputs
3. Validate environment configurations
4. Test locally with same steps

## Performance Optimization

### Build Speed
- **NuGet Caching**: Reduces dependency restore time
- **Docker Layer Caching**: Optimizes container builds  
- **Parallel Jobs**: Independent jobs run concurrently
- **Artifact Reuse**: Shared artifacts between jobs

### Resource Usage
- **Conditional Jobs**: Skip unnecessary builds
- **Timeout Limits**: Prevent runaway processes
- **Cleanup**: Automatic artifact retention management

## Future Enhancements

### Planned Features
- [ ] Automated load testing with NBomber
- [ ] Database migration validation
- [ ] Multi-environment deployment pipelines
- [ ] Performance benchmark tracking
- [ ] Advanced security scanning (SAST/DAST)
- [ ] Infrastructure as Code (IaC) validation

### Potential Integrations
- [ ] SonarCloud code quality analysis
- [ ] Slack/Teams notifications
- [ ] Jira issue tracking integration
- [ ] AWS/Azure deployment pipelines
- [ ] Kubernetes deployment manifests

## Best Practices

### Workflow Maintenance
1. **Regular Updates**: Keep action versions current
2. **Security Reviews**: Audit secrets and permissions quarterly
3. **Performance Monitoring**: Track build times and success rates
4. **Documentation**: Keep this document updated with changes

### Development Workflow
1. **Feature Branches**: Use feature branches for development
2. **Pull Requests**: All changes via PR review
3. **Testing**: Add tests for new features
4. **Documentation**: Update docs with feature changes

### Security Guidelines
1. **Secrets Rotation**: Rotate secrets regularly
2. **Principle of Least Privilege**: Minimal required permissions
3. **Dependency Updates**: Keep dependencies current
4. **Audit Logs**: Monitor workflow execution logs
