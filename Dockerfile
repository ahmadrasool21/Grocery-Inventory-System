# # --- Build stage ---
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /src

# # Copy sln and project files first for better layer caching
# COPY *.sln ./
# COPY src/GroceryInventory.Domain/*.csproj src/GroceryInventory.Domain/
# COPY src/GroceryInventory.Application/*.csproj src/GroceryInventory.Application/
# COPY src/GroceryInventory.Infrastructure/*.csproj src/GroceryInventory.Infrastructure/
# COPY src/GroceryInventory.Api/*.csproj src/GroceryInventory.Api/

# RUN dotnet restore

# # Copy the remaining source
# COPY . .

# # Publish API
# WORKDIR /src/src/GroceryInventory.Api
# RUN dotnet publish -c Release -o /app/publish --no-restore

# # --- Runtime stage ---
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
# WORKDIR /app
# COPY --from=build /app/publish .

# ENV ASPNETCORE_URLS=http://+:8080
# EXPOSE 8080

# ENTRYPOINT ["dotnet", "GroceryInventory.Api.dll"]





# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy sln and project files first for better layer caching
COPY *.sln ./

# Copy main project files
COPY src/GroceryInventory.Domain/*.csproj src/GroceryInventory.Domain/
COPY src/GroceryInventory.Application/*.csproj src/GroceryInventory.Application/
COPY src/GroceryInventory.Infrastructure/*.csproj src/GroceryInventory.Infrastructure/
COPY src/GroceryInventory.Api/*.csproj src/GroceryInventory.Api/

# Copy test project files (fix for missing projects)
COPY tests/GroceryInventory.UnitTests/*.csproj tests/GroceryInventory.UnitTests/
COPY tests/GroceryInventory.ApiTests/*.csproj tests/GroceryInventory.ApiTests/

# Restore dependencies
RUN dotnet restore

# Copy the remaining source
COPY . .

# Publish API
WORKDIR /src/src/GroceryInventory.Api
RUN dotnet publish -c Release -o /app/publish 

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GroceryInventory.Api.dll"]
