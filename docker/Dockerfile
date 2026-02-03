FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/EmployeeApi/EmployeeApi.csproj src/EmployeeApi/
RUN dotnet restore src/EmployeeApi/EmployeeApi.csproj
COPY src/EmployeeApi/ src/EmployeeApi/
RUN dotnet publish src/EmployeeApi/EmployeeApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
RUN useradd --create-home --shell /bin/bash appuser
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl --fail http://localhost:8080/health || exit 1
USER appuser
ENTRYPOINT ["dotnet", "EmployeeApi.dll"]
