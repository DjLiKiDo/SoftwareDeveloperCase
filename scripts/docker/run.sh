#!/bin/bash

# Docker Run Script for SoftwareDeveloperCase API
# Usage: ./scripts/docker/run.sh [tag] [port]

set -e

# Default values
DEFAULT_TAG="latest"
DEFAULT_PORT="8080"
IMAGE_NAME="softwaredevelopercase-api"
CONTAINER_NAME="softwaredevelopercase-api-container"

# Get parameters from arguments or use defaults
TAG=${1:-$DEFAULT_TAG}
PORT=${2:-$DEFAULT_PORT}
FULL_IMAGE_NAME="${IMAGE_NAME}:${TAG}"

echo "🐳 Running Docker container: ${CONTAINER_NAME}"
echo "🖼️  Image: ${FULL_IMAGE_NAME}"
echo "🔌 Port mapping: ${PORT}:8080"
echo ""

# Stop and remove existing container if it exists
if [ "$(docker ps -q -f name=${CONTAINER_NAME})" ]; then
    echo "🛑 Stopping existing container..."
    docker stop ${CONTAINER_NAME}
fi

if [ "$(docker ps -aq -f name=${CONTAINER_NAME})" ]; then
    echo "🗑️  Removing existing container..."
    docker rm ${CONTAINER_NAME}
fi

# Run the container
docker run \
    --name "${CONTAINER_NAME}" \
    -p "${PORT}:8080" \
    -d \
    "${FULL_IMAGE_NAME}"

echo ""
echo "✅ Container started successfully!"
echo "🌐 API available at: http://localhost:${PORT}"
echo "📋 Swagger UI: http://localhost:${PORT}/swagger"
echo ""
echo "📊 Container status:"
docker ps --filter name=${CONTAINER_NAME} --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

echo ""
echo "📝 Useful commands:"
echo "   View logs:    docker logs ${CONTAINER_NAME}"
echo "   Follow logs:  docker logs -f ${CONTAINER_NAME}"
echo "   Stop:         docker stop ${CONTAINER_NAME}"
echo "   Remove:       docker rm ${CONTAINER_NAME}"
