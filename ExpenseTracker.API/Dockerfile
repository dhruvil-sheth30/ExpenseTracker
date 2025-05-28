FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files for layer caching
COPY ["ExpenseTracker.API/ExpenseTracker.API.csproj", "ExpenseTracker.API/"]
COPY ["ExpenseTracker.Shared/ExpenseTracker.Shared.csproj", "ExpenseTracker.Shared/"]
RUN dotnet restore "ExpenseTracker.API/ExpenseTracker.API.csproj"

# Copy the rest of the code
COPY ["ExpenseTracker.API/", "ExpenseTracker.API/"]
COPY ["ExpenseTracker.Shared/", "ExpenseTracker.Shared/"]

# Build and publish
WORKDIR "/src/ExpenseTracker.API"
RUN dotnet build "ExpenseTracker.API.csproj" -c Release -o /app/build
RUN dotnet publish "ExpenseTracker.API.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ExpenseTracker.API.dll"]
