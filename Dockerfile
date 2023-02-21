#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MyHub.Api/MyHub.Api.csproj", "MyHub.Api/"]
COPY ["MyHub.Application/MyHub.Application.csproj", "MyHub.Application/"]
COPY ["MyHub.Domain/MyHub.Domain.csproj", "MyHub.Domain/"]
COPY ["MyHub.Infrastructure/MyHub.Infrastructure.csproj", "MyHub.Infrastructure/"]
RUN dotnet restore "MyHub.Api/MyHub.Api.csproj"
COPY . .
WORKDIR "/src/MyHub.Api"
RUN dotnet build "MyHub.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyHub.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyHub.Api.dll", "--environment=Production"]