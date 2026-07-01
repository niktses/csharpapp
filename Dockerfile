# Use the official .NET runtime as a parent image for the final run stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project files and restore dependencies
COPY ["src/CSharpApp.Api/CSharpApp.Api.csproj", "src/CSharpApp.Api/"]
COPY ["src/CSharpApp.Application/CSharpApp.Application.csproj", "src/CSharpApp.Application/"]
COPY ["src/CSharpApp.Core/CSharpApp.Core.csproj", "src/CSharpApp.Core/"]
COPY ["src/CSharpApp.Infrastructure/CSharpApp.Infrastructure.csproj", "src/CSharpApp.Infrastructure/"]
RUN dotnet restore "src/CSharpApp.Api/CSharpApp.Api.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/src/CSharpApp.Api"
RUN dotnet build "CSharpApp.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "CSharpApp.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage: copy published output and run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CSharpApp.Api.dll"]
