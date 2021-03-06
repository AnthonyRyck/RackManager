#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV DB_HOST="YourAddressdatabase"
ENV LOGIN_DB="YourLogin"
ENV PASSWORD_DB="YourPassword"
ENV DB_NAME="YourDbName"

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["RackCore/RackCore.csproj", "RackCore/"]
COPY ["RackManager/RackManager.csproj", "RackManager/"]
COPY ["AccessData/AccessData.csproj", "AccessData/"]

RUN dotnet restore "RackManager/RackManager.csproj"
COPY . .
WORKDIR "/src/RackManager"
RUN dotnet build "RackManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RackManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RackManager.dll"]