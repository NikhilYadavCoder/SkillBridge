# Stage 1: Copy pre-built frontend (dist folder already built locally)
FROM alpine:latest AS frontend-copy
WORKDIR /app
COPY Frontend/dist ./dist

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

# Copy pre-built frontend dist files to wwwroot
RUN mkdir -p wwwroot
COPY --from=frontend-copy /app/dist ./wwwroot

# Environment
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

# Start application
ENTRYPOINT ["dotnet", "SkillBridge.dll"]
