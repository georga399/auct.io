# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy the project files and restore dependencies
COPY src/Auctio.UserProfiles/Auctio.UserProfiles.csproj Auctio.UserProfiles/
COPY src/Auctio.Shared/Auctio.Shared.csproj Auctio.Shared/
RUN dotnet restore Auctio.UserProfiles/Auctio.UserProfiles.csproj

# Copy the remaining source code and build the app
COPY src/Auctio.UserProfiles/ Auctio.UserProfiles/
COPY src/Auctio.Shared/ Auctio.Shared/
RUN dotnet publish Auctio.UserProfiles/Auctio.UserProfiles.csproj -c Release -o /app/publish

# Use the official .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port the app runs on
EXPOSE 5002

# Set the entry point to run the app
ENTRYPOINT ["dotnet", "Auctio.UserProfiles.dll"]