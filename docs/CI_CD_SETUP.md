# CI/CD Setup Guide

This guide will help you configure the GitHub Actions CI/CD pipeline for the SoftwareDeveloperCase project.

## Quick Start

The CI/CD pipeline is ready to use out-of-the-box! Simply push your code to GitHub and the workflow will automatically:

✅ **Test your code** with comprehensive unit and integration tests  
✅ **Check code quality** with formatting and vulnerability scanning  
✅ **Build artifacts** for deployment  
✅ **Create Docker images** for containerized deployment  
✅ **Deploy to environments** (when configured)

## Prerequisites

- GitHub repository with the workflow files
- .NET 8 SDK (for local development)
- Git for version control

## Workflow Files Included

```
.github/
├── workflows/
│   ├── ci-cd.yml              # Main CI/CD pipeline
│   ├── codeql.yml             # Security code analysis
│   └── dependency-review.yml  # PR dependency scanning
└── renovate.json              # Automated dependency updates
```

## Configuration Steps

### 1. Required: None! 
The pipeline works immediately with default settings using:
- GitHub Container Registry (ghcr.io) for Docker images
- Built-in GitHub tokens for authentication
- Standard .NET build and test commands

### 2. Optional: Enhanced Features

#### Code Coverage (Optional)
To enable enhanced coverage reporting:
1. Sign up at [Codecov.io](https://codecov.io)
2. Add your repository
3. Add `CODECOV_TOKEN` secret in GitHub repository settings

#### Docker Hub (Optional)
To push to Docker Hub instead of GitHub Container Registry:
1. Add `DOCKER_USERNAME` secret with your Docker Hub username
2. Add `DOCKER_PASSWORD` secret with your Docker Hub token/password

#### Environment Protection (Recommended for Production)
1. Go to repository Settings → Environments
2. Create `staging` environment:
   - No protection rules (auto-deploy from develop branch)
3. Create `production` environment:
   - Add required reviewers
   - Restrict to main branch only

## Workflow Triggers

The pipeline automatically runs on:
- **Push** to `main` or `develop` branches
- **Pull requests** to `main` or `develop` branches  
- **Released** when you publish a GitHub release
- **Manual** trigger via GitHub Actions UI

## Pipeline Stages

### 🧪 Test & Code Quality (~3-5 min)
- Restores NuGet packages
- Verifies code formatting
- Runs unit tests with coverage
- Scans for vulnerabilities
- Uploads test results and coverage reports

### 🔨 Build & Package (~2-3 min)
- Builds release configuration
- Publishes API for deployment
- Uploads build artifacts

### 🔗 Integration Tests (~5-8 min)
- Starts SQL Server container
- Runs integration tests
- Validates database connectivity

### ⚡ Performance Tests (~3-5 min)
- Basic load testing (main branch only)
- API health validation

### 🐳 Docker Build (~4-6 min)
- Builds multi-stage Docker image
- Pushes to GitHub Container Registry
- Tags with branch name and SHA

### 🚀 Deploy (~1-2 min each)
- **Staging**: Auto-deploy from develop branch
- **Production**: Auto-deploy from main branch (with approval if environments configured)

## Local Testing

Test your changes locally before pushing:

```bash
# Build the solution
dotnet build

# Run tests
dotnet test

# Check code formatting
dotnet format --verify-no-changes

# Check for vulnerabilities
dotnet list package --vulnerable

# Build Docker image (if Docker installed)
docker build -t softwaredevelopercase:local -f src/SoftwareDeveloperCase.Api/Dockerfile .
```

## Monitoring Pipeline

### Success Indicators
- ✅ All jobs complete successfully
- ✅ Test coverage maintains >80%
- ✅ No vulnerable packages found
- ✅ Code formatting passes
- ✅ Docker image builds successfully

### Failure Troubleshooting

#### Build Failures
1. Check NuGet package compatibility
2. Verify .NET version requirements
3. Review compiler errors in logs
4. **NuGet caching issues**: If you see "Dependencies lock file is not found", the built-in caching is looking for `packages.lock.json`. Our workflow uses custom NuGet caching instead.

#### Test Failures  
1. Check SQL Server container startup
2. Review test database configuration
3. Validate test data setup

#### Docker Issues
1. Verify Dockerfile syntax
2. Check .dockerignore patterns
3. Review build context size

#### Format Issues
Run `dotnet format` locally to fix formatting

## Customization

### Adding New Tests
1. Add test projects to the solution
2. Tests will automatically run in the pipeline
3. Coverage reports will include new projects

### Changing Deploy Targets
1. Modify deployment steps in `ci-cd.yml`
2. Update environment configurations
3. Add necessary secrets for deployment

### Adding New Checks
1. Create new job in `ci-cd.yml`
2. Add dependencies with `needs:` keyword
3. Configure appropriate triggers

## Security Best Practices

### Secrets Management
- Never commit secrets to code
- Use GitHub repository secrets for sensitive data
- Rotate secrets regularly
- Use least-privilege access

### Dependency Management
- Keep dependencies updated (Renovate helps with this)
- Review dependency changes in PRs
- Monitor vulnerability reports

### Container Security
- Images run as non-root user
- Minimal base images used
- Regular base image updates

## Support

### Documentation
- [Full CI/CD Guide](./CI_CD_GUIDE.md) - Detailed technical documentation
- [GitHub Actions Docs](https://docs.github.com/en/actions) - Official GitHub documentation
- [.NET CLI Reference](https://docs.microsoft.com/en-us/dotnet/core/tools/) - .NET tooling

### Common Issues
- Check GitHub Actions logs for detailed error messages
- Verify required secrets are configured
- Ensure branch protection rules don't conflict
- Validate environment configurations

### Getting Help
1. Check the workflow logs in GitHub Actions tab
2. Review the troubleshooting section in CI_CD_GUIDE.md
3. Test locally to reproduce issues
4. Check GitHub Actions documentation for specific actions

---

**Ready to go!** 🚀 Push your code and watch the magic happen!
