# syntax=docker/dockerfile:1

# ── Stage 1: Build & publish ──────────────────────────────────────────────
# Builds the API and the Blazor WASM Web client from source. Because
# FosterFlow.Api references FosterFlow.Web and uses UseBlazorFrameworkFiles(),
# publishing the API compiles the Web client and copies its output into the
# API's wwwroot automatically.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first to leverage Docker layer caching for restore.
COPY FosterFlow/FosterFlow.sln ./FosterFlow/
COPY FosterFlow/FosterFlow.Api/FosterFlow.Api.csproj                       ./FosterFlow/FosterFlow.Api/
COPY FosterFlow/FosterFlow.Web/FosterFlow.Web.csproj                       ./FosterFlow/FosterFlow.Web/
COPY FosterFlow/FosterFlow.Infrastructure/FosterFlow.Infrastructure.csproj ./FosterFlow/FosterFlow.Infrastructure/
COPY FosterFlow/FosterFlow.Application/FosterFlow.Application.csproj        ./FosterFlow/FosterFlow.Application/
COPY FosterFlow/FosterFlow.Domain/FosterFlow.Domain.csproj                 ./FosterFlow/FosterFlow.Domain/
COPY FosterFlow/FosterFlow.Contracts/FosterFlow.Contracts.csproj           ./FosterFlow/FosterFlow.Contracts/
COPY FosterFlow/FosterFlow.Shared/FosterFlow.Shared.csproj                 ./FosterFlow/FosterFlow.Shared/

RUN dotnet restore FosterFlow/FosterFlow.Api/FosterFlow.Api.csproj

# Copy the rest of the source and publish.
COPY FosterFlow/ ./FosterFlow/
RUN dotnet publish FosterFlow/FosterFlow.Api/FosterFlow.Api.csproj \
    -c Release -o /app/publish --no-restore

# Build a self-contained EF Core migration bundle. This is an executable that
# applies the migrations to a database, so schema changes are applied as a
# separate deployment step instead of from application startup code.
RUN dotnet tool install --global dotnet-ef --version 10.* \
    && export PATH="$PATH:/root/.dotnet/tools" \
    && ConnectionStrings__Database="Server=placeholder;Database=placeholder;Trusted_Connection=False;Encrypt=False;" \
       dotnet ef migrations bundle \
        --project FosterFlow/FosterFlow.Infrastructure/FosterFlow.Infrastructure.csproj \
        --startup-project FosterFlow/FosterFlow.Api/FosterFlow.Api.csproj \
        --configuration Release \
        --self-contained \
        -o /app/efbundle

# ── Stage 2: Migrator ─────────────────────────────────────────────────────
# A tiny image whose only job is to run the migration bundle and exit. Used as
# a one-off init step (see the `migrator` service in docker-compose.yml).
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS migrator
WORKDIR /app
COPY --from=build /app/efbundle ./efbundle
ENTRYPOINT ["./efbundle"]

# ── Stage 3: Runtime (API) ────────────────────────────────────────────────
# Last stage so `docker build -t fosterflow-api .` produces the API image.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "FosterFlow.Api.dll"]
