#!/bin/bash

# Docker Logs Script for SoftwareDeveloperCase API
# Usage: ./scripts/docker/logs.sh [follow]

set -e

CONTAINER_NAME="softwaredevelopercase-api-container"
FOLLOW_FLAG=${1:-""}

echo "📋 Viewing logs for container: ${CONTAINER_NAME}"
echo ""

# Check if container exists
if [ ! "$(docker ps -q -f name=${CONTAINER_NAME})" ]; then
    if [ ! "$(docker ps -aq -f name=${CONTAINER_NAME})" ]; then
        echo "❌ Container '${CONTAINER_NAME}' does not exist."
        echo "💡 Run './scripts/docker/run.sh' to start the container first."
        exit 1
    else
        echo "⚠️  Container '${CONTAINER_NAME}' is not running."
        echo "💡 Start it with: docker start ${CONTAINER_NAME}"
        echo "📋 Showing logs from last run:"
        echo ""
    fi
fi

# Show logs
if [ "$FOLLOW_FLAG" = "follow" ] || [ "$FOLLOW_FLAG" = "-f" ]; then
    echo "👀 Following logs (Press Ctrl+C to stop)..."
    echo ""
    docker logs -f ${CONTAINER_NAME}
else
    echo "📜 Recent logs:"
    echo ""
    docker logs --tail 50 ${CONTAINER_NAME}
    echo ""
    echo "💡 To follow logs in real-time: ./scripts/docker/logs.sh follow"
fi
