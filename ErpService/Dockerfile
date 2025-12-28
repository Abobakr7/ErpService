FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ErpService.csproj", "./"]
RUN dotnet restore "ErpService.csproj"
COPY . .

RUN dotnet build "ErpService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ErpService.csproj" -c Release -o /app/publish \
    /p:UseAppHost=false \
    /p:PublishTrimmed=false \
    /p:PublishSingleFile=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app


RUN groupadd -r appuser && useradd -r -g appuser appuser \
    && chown -R appuser:appuser /app


COPY --chown=appuser:appuser --from=publish /app/publish .


USER appuser

EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "ErpService.dll"]