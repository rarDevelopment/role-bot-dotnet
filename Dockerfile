#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RoleBot/RoleBot.csproj", "RoleBot/"]
COPY ["RoleBot.Models/RoleBot.Models.csproj", "RoleBot.Models/"]
RUN dotnet restore "RoleBot/RoleBot.csproj"
COPY . .
WORKDIR "/src/RoleBot"
RUN dotnet build "RoleBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RoleBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RoleBot.dll"]