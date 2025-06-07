#!/bin/bash

# Docker Stop and Clean Script for SoftwareDeveloperCase API
# Usage: ./scripts/docker/stop.sh [clean]

set -e

CONTAINER_NAME="softwaredevelopercase-api-container"
IMAGE_NAME="softwaredevelopercase-api"
CLEAN_FLAG=${1:-""}

echo "🛑 Stopping Docker container: ${CONTAINER_NAME}"
echo ""

# Stop container if running
if [ "$(docker ps -q -f name=${CONTAINER_NAME})" ]; then
    echo "⏹️  Stopping container..."
    docker stop ${CONTAINER_NAME}
    echo "✅ Container stopped successfully"
else
    echo "ℹ️  Container is not running"
fi

# Remove container if exists
if [ "$(docker ps -aq -f name=${CONTAINER_NAME})" ]; then
    echo "🗑️  Removing container..."
    docker rm ${CONTAINER_NAME}
    echo "✅ Container removed successfully"
else
    echo "ℹ️  Container does not exist"
fi

# Clean images if requested
if [ "$CLEAN_FLAG" = "clean" ] || [ "$CLEAN_FLAG" = "--clean" ]; then
    echo ""
    echo "🧹 Cleaning up Docker images..."
    
    # Remove project images
    if [ "$(docker images -q ${IMAGE_NAME})" ]; then
        echo "🗑️  Removing ${IMAGE_NAME} images..."
        docker rmi $(docker images ${IMAGE_NAME} -q) 2>/dev/null || true
        echo "✅ Project images removed"
    else
        echo "ℹ️  No project images to remove"
    fi
    
    # Remove dangling images
    if [ "$(docker images -f dangling=true -q)" ]; then
        echo "🗑️  Removing dangling images..."
        docker image prune -f
        echo "✅ Dangling images removed"
    else
        echo "ℹ️  No dangling images to remove"
    fi
fi

echo ""
echo "✅ Cleanup completed!"
echo ""
echo "💡 Useful commands:"
echo "   View all containers: docker ps -a"
echo "   View all images:     docker images"
echo "   Clean system:        docker system prune"
