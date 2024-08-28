# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the csproj file(s) and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the entire project and build it
COPY . ./
RUN dotnet publish -c Release -o /publish

# 2. Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the build output from the build stage
COPY --from=build /publish .

# Expose the port your app runs on (typically 80)
EXPOSE 5229

# Set the entry point to run your application
ENTRYPOINT ["Moriarty.Web"]
