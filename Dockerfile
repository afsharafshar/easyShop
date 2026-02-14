FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ["src/EasyShop.Api/EasyShop.Api.csproj", "src/EasyShop.Api/"]
COPY ["src/EasyShop.Infrastructure/EasyShop.Infrastructure.csproj", "src/EasyShop.Infrastructure/"]
COPY ["src/EasyShop.Application/EasyShop.Application.csproj", "src/EasyShop.Application/"]
COPY ["src/EasyShop.Domain/EasyShop.Domain.csproj", "src/EasyShop.Domain/"]
RUN dotnet restore "src/EasyShop.Api/EasyShop.Api.csproj"
COPY . .
WORKDIR "/src/src/EasyShop.Api"
RUN dotnet build "./EasyShop.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EasyShop.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyShop.Api.dll"]
