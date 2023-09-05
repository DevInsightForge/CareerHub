FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CareerHub.gRPC/CareerHub.gRPC.csproj", "CareerHub.gRPC/"]
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
