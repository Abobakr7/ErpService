# ===========================================
# Stage 1: Build Stage
# ===========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["ErpService.csproj", "./"]
RUN dotnet restore "ErpService.csproj"

# Copy all source files
COPY . .

# Build the application
RUN dotnet build "ErpService.csproj" -c Release -o /app/build

# ===========================================
# Stage 2: Publish Stage
# ===========================================
FROM build AS publish
RUN dotnet publish "ErpService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===========================================
# Stage 3: Runtime Stage (Final Image)
# ===========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install curl for health checks
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Expose port (ASP.NET Core uses 8080 by default in .NET 8+)
EXPOSE 8080
EXPOSE 8081

# Health check endpoint - Fixed path ✅
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/api/invoices/Check || exit 1

# Set entry point
ENTRYPOINT ["dotnet", "ErpService.dll"]