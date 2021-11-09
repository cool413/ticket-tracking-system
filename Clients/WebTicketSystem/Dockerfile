#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 8030

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Models/Entities/Entities.csproj", "Models/Entities/"]
COPY ["Models/Common/Common.csproj", "Models/Common/"]
COPY ["Models/Messages/Messages.csproj", "Models/Messages/"]
COPY ["Models/DTOs/DTOs.csproj", "Models/DTOs/"]
COPY ["Services/Infrastructure/Infrastructure.csproj", "Services/Infrastructure/"]
COPY ["Services/DataCommon/DataCommon.csproj", "Services/DataCommon/"]
COPY ["Services/TicketSystemService/TicketSystemService.csproj", "Services/TicketSystemService/"]
COPY ["Databases/TicketSystemContext/TicketSystemContext.csproj", "Databases/TicketSystemContext/"]
COPY ["Clients/WebTicketSystem/WebTicketSystem.csproj", "Clients/WebTicketSystem/"]

RUN dotnet restore "Clients/WebTicketSystem/WebTicketSystem.csproj"

COPY . .
WORKDIR /src/Clients/WebTicketSystem
RUN dotnet build "WebTicketSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebTicketSystem.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Clients.WebTicketSystem.dll"]