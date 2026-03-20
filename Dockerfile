# Stage 1: Build frontend
FROM node:18-alpine AS frontend-build
WORKDIR /app/frontend
COPY Frontend/package*.json ./
RUN npm install
COPY Frontend/ .
RUN npm run build

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
