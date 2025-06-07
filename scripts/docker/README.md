# Docker Scripts README

This directory contains utility scripts for Docker operations in the SoftwareDeveloperCase project.

## Scripts Overview

### 🔨 build.sh

Builds the Docker image for the API.

**Usage:**

```bash
./scripts/docker/build.sh [tag]
```

**Examples:**

```bash
./scripts/docker/build.sh          # Builds with 'latest' tag
./scripts/docker/build.sh v1.0.0   # Builds with 'v1.0.0' tag
./scripts/docker/build.sh dev      # Builds with 'dev' tag
```

### 🚀 run.sh

Runs the Docker container with the API.

**Usage:**

```bash
./scripts/docker/run.sh [tag] [port]
```

**Examples:**

```bash
./scripts/docker/run.sh                    # Runs latest:8080
./scripts/docker/run.sh latest 8081        # Runs latest:8081
./scripts/docker/run.sh dev 3000           # Runs dev:3000
```

### 📋 logs.sh

Views container logs.

**Usage:**

```bash
./scripts/docker/logs.sh [follow]
```

**Examples:**

```bash
./scripts/docker/logs.sh           # Shows recent logs
./scripts/docker/logs.sh follow    # Follows logs in real-time
./scripts/docker/logs.sh -f        # Same as follow
```

### 🛑 stop.sh

Stops and optionally cleans up Docker resources.

**Usage:**

```bash
./scripts/docker/stop.sh [clean]
```

**Examples:**

```bash
./scripts/docker/stop.sh           # Stops and removes container
./scripts/docker/stop.sh clean     # Also removes images and cleanup
./scripts/docker/stop.sh --clean   # Same as clean
```

## Quick Workflow

### Development Workflow

```bash
# Build the image
./scripts/docker/build.sh dev

# Run the container
./scripts/docker/run.sh dev 8080

# View logs
./scripts/docker/logs.sh follow

# Stop when done
./scripts/docker/stop.sh
```

### Production Build

```bash
# Build production image
./scripts/docker/build.sh v1.0.0

# Test run
./scripts/docker/run.sh v1.0.0 8080

# Check logs
./scripts/docker/logs.sh

# Clean up
./scripts/docker/stop.sh clean
```

## Prerequisites

- Docker Desktop installed and running
- Execute permissions on scripts: `chmod +x scripts/docker/*.sh`

## Troubleshooting

### Script Permission Issues

```bash
chmod +x scripts/docker/*.sh
```

### Container Already Running

The scripts automatically handle stopping existing containers before starting new ones.

### Port Already in Use

Use a different port: `./scripts/docker/run.sh latest 8081`

### Clean Everything

```bash
./scripts/docker/stop.sh clean
docker system prune -af  # Nuclear option - removes everything
```
