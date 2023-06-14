# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Base image for ASP.NET Core app
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build image for ASP.NET Core app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["open-food-api/open-food-api.csproj", "open-food-api/"]
RUN dotnet restore "open-food-api/open-food-api.csproj"
COPY . .
WORKDIR "/src/open-food-api"
RUN dotnet build "open-food-api.csproj" -c Release -o /app/build

# Publish image for ASP.NET Core app
FROM build AS publish
RUN dotnet publish "open-food-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Base image for MongoDB
FROM mongo:latest AS mongodb

# Final image
FROM base AS final
WORKDIR /app

# Copy the published ASP.NET Core app
COPY --from=publish /app/publish .

# Set the entry point to start the ASP.NET Core app
ENTRYPOINT ["dotnet", "open-food-api.dll"]
