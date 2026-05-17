# syntax=docker/dockerfile:1.7
# Build context must be the repository root.

ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props ./

COPY src/Lab5.Tools/Lab5.Tools.csproj src/Lab5.Tools/
COPY src/Application/Lab5.Tools.Application/Lab5.Tools.Application.csproj src/Application/Lab5.Tools.Application/
COPY src/Application/Lab5.Tools.Application.Abstractions/Lab5.Tools.Application.Abstractions.csproj src/Application/Lab5.Tools.Application.Abstractions/
COPY src/Application/Lab5.Tools.Application.Contracts/Lab5.Tools.Application.Contracts.csproj src/Application/Lab5.Tools.Application.Contracts/
COPY src/Application/Lab5.Tools.Application.Models/Lab5.Tools.Application.Models.csproj src/Application/Lab5.Tools.Application.Models/
COPY src/Infrastructure/Lab5.Tools.Infrastructure.Persistence/Lab5.Tools.Infrastructure.Persistence.csproj src/Infrastructure/Lab5.Tools.Infrastructure.Persistence/
COPY src/Infrastructure/Lab5.Tools.Infrastructure.Kafka/Lab5.Tools.Infrastructure.Kafka.csproj src/Infrastructure/Lab5.Tools.Infrastructure.Kafka/
COPY src/Infrastructure/Lab5.Tools.Infrastructure.Integrations/Lab5.Tools.Infrastructure.Integrations.csproj src/Infrastructure/Lab5.Tools.Infrastructure.Integrations/
COPY src/Presentation/Lab5.Tools.Presentation.Grpc/Lab5.Tools.Presentation.Grpc.csproj src/Presentation/Lab5.Tools.Presentation.Grpc/
COPY src/Presentation/Lab5.Tools.Presentation.Kafka/Lab5.Tools.Presentation.Kafka.csproj src/Presentation/Lab5.Tools.Presentation.Kafka/

RUN dotnet restore src/Lab5.Tools/Lab5.Tools.csproj

COPY src/ src/

RUN dotnet publish src/Lab5.Tools/Lab5.Tools.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8070

EXPOSE 8070

ENTRYPOINT ["dotnet", "Lab5.Tools.dll"]
