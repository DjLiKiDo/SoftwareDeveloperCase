#!/bin/bash
if [ -z "$1" ]; then
  echo "Usage: ./add-migration.sh <MigrationName>"
  exit 1
fi
echo "📦 Adding migration: $1"
dotnet ef migrations add "$1" \
  --project src/SoftwareDeveloperCase.Infrastructure \
  --startup-project src/SoftwareDeveloperCase.Api
