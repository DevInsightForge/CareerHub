FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ["CareerHub.gRPC/CareerHub.gRPC.csproj", "CareerHub.gRPC/"]
COPY ["CareerHub.Domain/CareerHub.Domain.csproj", "CareerHub.Domain/"]
COPY ["CareerHub.Application/CareerHub.Application.csproj", "CareerHub.Application/"]
COPY ["CareerHub.Infrastructure/CareerHub.Infrastructure.csproj", "CareerHub.Infrastructure/"]
RUN dotnet restore "CareerHub.gRPC/CareerHub.gRPC.csproj"
COPY . .
WORKDIR "/src/CareerHub.gRPC"
RUN dotnet build "CareerHub.gRPC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CareerHub.gRPC.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CareerHub.gRPC.dll"]
