# Stage 1: Build frontend
FROM node:18-alpine AS frontend-build
WORKDIR /app/frontend
# Copy package files
COPY Frontend/package.json Frontend/package-lock.json ./
# Install dependencies with clean install (uses package-lock.json)
RUN npm ci --prefer-offline --no-audit --verbose
# Copy source code
COPY Frontend/ .
# List files for debugging
RUN ls -la && echo "=== Building frontend ===" 
# Build with verbose output
RUN npm run build --verbose 2>&1

# Stage 2: Build backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /src
COPY SkillBridge.csproj ./
RUN dotnet restore "SkillBridge.csproj"
COPY . .
RUN dotnet build "SkillBridge.csproj" -c Release -o /app/build

# Stage 3: Publish backend
FROM backend-build AS publish
RUN dotnet publish "SkillBridge.csproj" -c Release -o /app/publish

# Stage 4: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published backend
COPY --from=publish /app/publish .

# Copy frontend dist files to wwwroot
RUN mkdir -p wwwroot
COPY --from=frontend-build /app/frontend/dist ./wwwroot

# Environment
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

# Start application
ENTRYPOINT ["dotnet", "SkillBridge.dll"]
