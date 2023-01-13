#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PostalTracker.Orchestrator.Host/PostalTracker.Orchestrator.Host.csproj", "PostalTracker.Orchestrator.Host/"]
COPY ["PostalTracker.Contracts/PostalTracker.Contracts.csproj", "PostalTracker.Contracts/"]
RUN dotnet restore "PostalTracker.Orchestrator.Host/PostalTracker.Orchestrator.Host.csproj"
COPY . .
WORKDIR "/src/PostalTracker.Orchestrator.Host"
RUN dotnet build "PostalTracker.Orchestrator.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PostalTracker.Orchestrator.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostalTracker.Orchestrator.Host.dll"]