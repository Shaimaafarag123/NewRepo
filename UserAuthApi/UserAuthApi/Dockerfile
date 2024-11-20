# Use the official .NET SDK image from the Microsoft Container Registry
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory in the container
WORKDIR /src

# Copy the project file and restore dependencies
COPY UserAuthApi.csproj ./UserAuthApi/
RUN dotnet restore "./UserAuthApi/UserAuthApi.csproj"

# Copy the rest of the application code
COPY . .

# Build the project
RUN dotnet publish "UserAuthApi/UserAuthApi.csproj" -c Release -o /app

# Define the entry point for the container
ENTRYPOINT ["dotnet", "/app/UserAuthApi.dll"]
