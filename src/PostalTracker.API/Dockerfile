#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PostalTracker.API/PostalTracker.API.csproj", "PostalTracker.API/"]
COPY ["PostalTracker.Contracts/PostalTracker.Contracts.csproj", "PostalTracker.Contracts/"]
COPY ["PostalTracker.System/PostalTracker.System.csproj", "PostalTracker.System/"]
RUN dotnet restore "PostalTracker.API/PostalTracker.API.csproj"
COPY . .
WORKDIR "/src/PostalTracker.API"
RUN dotnet build "PostalTracker.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PostalTracker.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostalTracker.API.dll"]