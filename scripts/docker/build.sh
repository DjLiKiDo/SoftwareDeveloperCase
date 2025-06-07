#!/bin/bash

# Docker Build Script for SoftwareDeveloperCase API
# Usage: ./scripts/docker/build.sh [tag]

set -e

# Default values
DEFAULT_TAG="latest"
DOCKERFILE_PATH="src/SoftwareDeveloperCase.Api/Dockerfile"
IMAGE_NAME="softwaredevelopercase-api"

# Get tag from argument or use default
TAG=${1:-$DEFAULT_TAG}
FULL_IMAGE_NAME="${IMAGE_NAME}:${TAG}"

echo "🐳 Building Docker image: ${FULL_IMAGE_NAME}"
echo "📁 Using Dockerfile: ${DOCKERFILE_PATH}"
echo "📂 Build context: $(pwd)"
echo ""

# Build the Docker image
docker build \
    -f "${DOCKERFILE_PATH}" \
    -t "${FULL_IMAGE_NAME}" \
    .

echo ""
echo "✅ Docker image built successfully: ${FULL_IMAGE_NAME}"
echo "📊 Image size:"
docker images "${IMAGE_NAME}" --format "table {{.Repository}}\t{{.Tag}}\t{{.Size}}\t{{.CreatedAt}}"

echo ""
echo "🚀 To run the container:"
echo "   docker run -p 8080:8080 ${FULL_IMAGE_NAME}"
