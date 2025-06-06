#!/bin/bash
echo "🗄️ Updating database..."
dotnet ef database update \
  --project src/SoftwareDeveloperCase.Infrastructure \
  --startup-project src/SoftwareDeveloperCase.Api
