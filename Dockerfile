# Use the official .NET 8.0 SDK image as the base image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:2c7794fef0107c1815193dc157ed3f8ca60eb504a439806a29b6f9bf1ce73679 AS build
# Set the working directory in the container
WORKDIR /src

# Copy everything into the container at /src
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish the application
RUN dotnet publish -c Release -p:PublishDir=out

# Use the official .NET 8.0 runtime image as the base image for running the application
FROM mcr.microsoft.com/dotnet/runtime:8.0@sha256:b0b169cd80db972dd185f7e931551dea3d665935aee60ec10616cc4ba59217f0
# Set the working directory in the container
WORKDIR /App
# Copy the published application from the build stage
COPY --from=build /src/out .
# Set the entry point for the application
ENTRYPOINT ["dotnet", "mapperPizelScan.dll"]