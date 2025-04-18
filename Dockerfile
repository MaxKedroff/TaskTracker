# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["TaskTracker.csproj", "."]
RUN dotnet restore "TaskTracker.csproj"
COPY . .
RUN dotnet build "TaskTracker.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "TaskTracker.csproj" -c Release -o /app/publish

# Migration stage (новый этап)
FROM build AS migrations
WORKDIR /src
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet ef migrations bundle --self-contained -r linux-x64 --output /app/migrations-bundle

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080;https://+:443
ENTRYPOINT ["dotnet", "TaskTracker.dll"]