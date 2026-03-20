# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["SkillBridge.csproj", "./"]
RUN dotnet restore "SkillBridge.csproj"

# Copy source and build
COPY . .
RUN dotnet build "SkillBridge.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SkillBridge.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

# Expose port (Render will override this with PORT env var)
EXPOSE 8080

# Environment - use PORT env var from Render, default to 8080
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

# Start application
ENTRYPOINT ["dotnet", "SkillBridge.dll"]
