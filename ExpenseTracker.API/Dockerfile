FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["ExpenseTracker.API/ExpenseTracker.API.csproj", "ExpenseTracker.API/"]
COPY ["ExpenseTracker.Shared/ExpenseTracker.Shared.csproj", "ExpenseTracker.Shared/"]

# Restore dependencies
RUN dotnet restore "ExpenseTracker.API/ExpenseTracker.API.csproj"

# Copy the rest of the code
COPY ["ExpenseTracker.API/", "ExpenseTracker.API/"]
COPY ["ExpenseTracker.Shared/", "ExpenseTracker.Shared/"]

# Build and publish
RUN dotnet build "ExpenseTracker.API/ExpenseTracker.API.csproj" -c Release -o /app/build
RUN dotnet publish "ExpenseTracker.API/ExpenseTracker.API.csproj" -c Release -o /app/publish

# Generate runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ExpenseTracker.API.dll"]ENTRYPOINT ["dotnet", "ExpenseTracker.API.dll"]
